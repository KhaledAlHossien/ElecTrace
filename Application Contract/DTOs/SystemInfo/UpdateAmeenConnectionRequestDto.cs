namespace Application_Contract.DTOs.SystemInfo
{
    // Password فارغة = الإبقاء على كلمة المرور المحفوظة
    public record UpdateAmeenConnectionRequestDto(string Server, string Database, string UserId, string? Password);
}
