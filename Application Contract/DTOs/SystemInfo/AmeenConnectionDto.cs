namespace Application_Contract.DTOs.SystemInfo
{
    // كلمة المرور ما بترجع أبداً للواجهة، بس منبيّن إذا كانت محفوظة
    public record AmeenConnectionDto(string Server, string Database, string UserId, bool HasPassword, bool IsConfigured);
}
