namespace UI.Services;

public enum ToastLevel
{
    Success,
    Error,
    Warning,
    Info
}

public sealed class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Text { get; init; } = "";
    public ToastLevel Level { get; init; }
    public bool IsLeaving { get; set; }
}

public sealed class ToastService
{
    private readonly List<ToastMessage> _toasts = [];

    public event Action? OnChange;

    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public void Success(string text) => Show(text, ToastLevel.Success);
    public void Error(string text) => Show(text, ToastLevel.Error);
    public void Warning(string text) => Show(text, ToastLevel.Warning);
    public void Info(string text) => Show(text, ToastLevel.Info);

    public void Show(string text, ToastLevel level = ToastLevel.Info, int durationMs = 4000)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var toast = new ToastMessage { Text = text, Level = level };
        _toasts.Add(toast);
        OnChange?.Invoke();

        _ = AutoDismissAsync(toast, durationMs);
    }

    public async Task DismissAsync(ToastMessage toast)
    {
        if (toast.IsLeaving)
            return;

        // يخلي الإشعار يطلع بحركة خروج قبل ما ينحذف فعلياً من القائمة
        toast.IsLeaving = true;
        OnChange?.Invoke();

        await Task.Delay(260);
        _toasts.Remove(toast);
        OnChange?.Invoke();
    }

    private async Task AutoDismissAsync(ToastMessage toast, int durationMs)
    {
        await Task.Delay(durationMs);
        await DismissAsync(toast);
    }
}
