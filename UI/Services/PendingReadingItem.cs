using Domain.Enums;

namespace UI.Services;

// قراءة أُدخلت أثناء انقطاع الاتصال (أو فشل الإرسال) ومحفوظة محلياً بانتظار المزامنة مع السيرفر
public sealed class PendingReadingItem
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string MeterCode { get; init; } = "";
    public string DepartmentName { get; init; } = "";
    public Months Month { get; init; }
    public int Year { get; init; }
    public decimal CurrentReading { get; init; }
    public bool ForceDuplicate { get; set; }
    public DateTime QueuedAt { get; init; } = DateTime.Now;

    // null = لسا ما تمت محاولة مزامنتها، أو آخر محاولة نجحت مؤقتاً بس فشلت لاحقاً بسبب انقطاع الشبكة (يُعاد المحاولة تلقائياً)
    // قيمة غير null = خطأ يتطلب قرار من المستخدم (مثل تكرار القراءة) ولازم تدخّل يدوي
    public string? LastError { get; set; }
    public int AttemptCount { get; set; }
}
