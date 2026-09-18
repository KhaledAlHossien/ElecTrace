namespace UI.Services;

public sealed class ConfirmOptions
{
    public string Title { get; init; } = "تأكيد العملية";
    public string Message { get; init; } = "هل أنت متأكد؟";
    public string ConfirmText { get; init; } = "تأكيد";
    public string CancelText { get; init; } = "إلغاء";
    public bool IsDanger { get; init; } = true;
}

public sealed class DialogService
{
    private TaskCompletionSource<bool>? _pending;

    public event Action<ConfirmOptions>? OnOpen;

    public Task<bool> ConfirmAsync(ConfirmOptions options)
    {
        // لو في نافذة مفتوحة من قبل منسكرها كأنها انلغت حتى ما يضل طلب معلّق للأبد
        _pending?.TrySetResult(false);

        _pending = new TaskCompletionSource<bool>();
        OnOpen?.Invoke(options);
        return _pending.Task;
    }

    public Task<bool> ConfirmDeleteAsync(string itemName) => ConfirmAsync(new ConfirmOptions
    {
        Title = "تأكيد الحذف",
        Message = $"سيتم حذف \"{itemName}\" نهائياً. لا يمكن التراجع عن هذه العملية.",
        ConfirmText = "نعم، احذف",
        CancelText = "تراجع",
        IsDanger = true
    });

    public void Complete(bool result)
    {
        _pending?.TrySetResult(result);
        _pending = null;
    }
}
