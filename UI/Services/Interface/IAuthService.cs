using Application_Contract.DTOs.User;

namespace UI.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}
