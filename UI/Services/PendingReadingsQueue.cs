using Application_Contract.DTOs.MeterReading;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using UI.Services.Interface;

namespace UI.Services;

// طابور القراءات اللي فشل إرسالها للسيرفر (غالباً بسبب انقطاع الشبكة) ومحفوظة بالمتصفح بانتظار المزامنة.
// بيحاول المزامنة تلقائياً: فور استعادة الاتصال (حدث "online")، وبشكل دوري كل ربع دقيقة كاحتياط
// (navigator.onLine مو موثوق 100%، ممكن يرجع true رغم عدم وجود إنترنت فعلي خلف الشبكة المحلية).
public sealed class PendingReadingsQueue : IAsyncDisposable
{
    private const string StorageKey = "pendingMeterReadings";
    private const int RetryIntervalMs = 20_000;

    private readonly ILocalStorageService _localStorage;
    private readonly IApiDataService _api;
    private readonly ToastService _toasts;
    private readonly IJSRuntime _js;

    private List<PendingReadingItem> _items = [];
    private bool _loaded;
    private bool _initialized;
    private DotNetObjectReference<PendingReadingsQueue>? _selfRef;
    private Timer? _retryTimer;

    public PendingReadingsQueue(ILocalStorageService localStorage, IApiDataService api, ToastService toasts, IJSRuntime js)
    {
        _localStorage = localStorage;
        _api = api;
        _toasts = toasts;
        _js = js;
    }

    // بينادى كل ما تغيّرت القائمة (إضافة/حذف/بداية أو نهاية مزامنة) حتى تحدّث أي واجهة مشتركة (شارة القائمة الجانبية، صفحة المزامنة)
    public event Action? Changed;

    public bool IsSyncing { get; private set; }
    public IReadOnlyList<PendingReadingItem> Items => _items;

    // بانتظار الدور (لسا ما جُرّبت، أو فشلت بسبب انقطاع مؤقت وبتنعاد تلقائياً)
    public int PendingCount => _items.Count(i => i.LastError is null);

    // فشلت بخطأ من السيرفر نفسه (تكرار قراءة، قسم محذوف...) وبتحتاج قرار يدوي بصفحة المزامنة
    public int AttentionCount => _items.Count(i => i.LastError is not null);

    public int TotalCount => _items.Count;

    // تُستدعى مرة وحدة من التخطيط الرئيسي بعد أول رسم، لتحميل القائمة المحفوظة وتفعيل المزامنة التلقائية
    public async Task InitializeAsync()
    {
        if (_initialized)
            return;
        _initialized = true;

        await EnsureLoadedAsync();

        // لازم نبلّغ المشتركين (شارة القائمة، صفحة المزامنة) بأي قراءات محفوظة من جلسة سابقة فوراً،
        // خصوصاً لو كلها متوقفة بانتظار قرار يدوي (LastError != null) وما رح تستدعي SyncAsync تلقائياً تحت
        Changed?.Invoke();

        _selfRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("netStatus.registerCallbacks", _selfRef);

        _retryTimer = new Timer(async _ => await TrySyncSilentlyAsync(), null, RetryIntervalMs, RetryIntervalMs);

        if (_items.Any(i => i.LastError is null))
            _ = SyncAsync();
    }

    [JSInvokable]
    public Task OnOnline()
    {
        _ = SyncAsync();
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task OnOffline() => Task.CompletedTask;

    private async Task TrySyncSilentlyAsync()
    {
        try { await SyncAsync(); }
        catch { /* رح تنعاد المحاولة بالدورة الجاية تلقائياً */ }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_loaded)
            return;

        _items = await _localStorage.GetItemAsync<List<PendingReadingItem>>(StorageKey) ?? [];
        _loaded = true;
    }

    public async Task EnqueueAsync(PendingReadingItem item)
    {
        await EnsureLoadedAsync();
        _items.Add(item);
        await PersistAsync();
        Changed?.Invoke();

        // محاولة فورية: ربما كان الفشل الأصلي عرضياً (انقطاع لحظي) والاتصال فعلياً موجود
        _ = SyncAsync();
    }

    public async Task RemoveAsync(string id)
    {
        await EnsureLoadedAsync();
        _items.RemoveAll(i => i.Id == id);
        await PersistAsync();
        Changed?.Invoke();
    }

    // إعادة محاولة قراءة متوقفة بانتظار قرار يدوي؛ force = تجاوز تحذير التكرار وتسجيلها كقراءة إضافية
    public async Task RetryAsync(string id, bool force)
    {
        await EnsureLoadedAsync();
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item is null)
            return;

        item.ForceDuplicate = force || item.ForceDuplicate;
        item.LastError = null;
        await PersistAsync();
        Changed?.Invoke();

        await SyncAsync();
    }

    public async Task SyncAsync()
    {
        await EnsureLoadedAsync();

        if (IsSyncing || !_items.Any(i => i.LastError is null))
            return;

        IsSyncing = true;
        Changed?.Invoke();

        try
        {
            foreach (var item in _items.Where(i => i.LastError is null).ToList())
            {
                try
                {
                    await _api.CreateMeterReadingAsync(new CreateMeterReadingRequestDto(item.MeterCode, item.Month, item.CurrentReading, item.ForceDuplicate));
                    _items.Remove(item);
                    _toasts.Success($"تمت مزامنة قراءة \"{item.DepartmentName}\" بعد استعادة الاتصال.");
                }
                catch (HttpRequestException)
                {
                    // ما زلنا بدون اتصال فعلي بالسيرفر: منوقف عن باقي القائمة، رح تنعاد المحاولة تلقائياً
                    break;
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // خطأ من السيرفر نفسه (تكرار، قسم محذوف...) بيحتاج قرار يدوي، فمنسجّله ومنكمل الباقي
                    item.AttemptCount++;
                    item.LastError = ex.Message;
                }
            }

            await PersistAsync();
        }
        finally
        {
            IsSyncing = false;
            Changed?.Invoke();
        }
    }

    private ValueTask PersistAsync() => _localStorage.SetItemAsync(StorageKey, _items);

    public ValueTask DisposeAsync()
    {
        _retryTimer?.Dispose();
        _selfRef?.Dispose();
        return ValueTask.CompletedTask;
    }
}
