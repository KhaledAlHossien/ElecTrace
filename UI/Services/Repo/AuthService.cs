using Application_Contract.DTOs.User;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using UI.Services.Interface;

namespace UI.Services.Repo
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";
        private const string UserNameKey = "userName";
        private const string FullNameKey = "fullName";

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null && !string.IsNullOrEmpty(authResult.Token))
                {
                    await _localStorage.SetItemAsync(TokenKey, authResult.Token);
                    await _localStorage.SetItemAsync(UserNameKey, authResult.UserName);
                    await _localStorage.SetItemAsync(FullNameKey, authResult.FullName);
                }
                return authResult!;
            }

            // لو فشل الـ Login، نقرأ الرسالة من الـ API إن وجدت
            var error = await response.Content.ReadAsStringAsync();
            throw new UnauthorizedAccessException(error);
        }

        public async Task LogoutAsync()
        {
            // محاولة إعلام API بأن التوكن سيتم إبطاله
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Auth/Logout");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                await _httpClient.SendAsync(request);
            }

            // مسح البيانات من LocalStorage بغض النظر عن نتيجة الـ API
            await _localStorage.RemoveItemAsync(TokenKey);
            await _localStorage.RemoveItemAsync(UserNameKey);
            await _localStorage.RemoveItemAsync(FullNameKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            return !string.IsNullOrEmpty(token);
        }
    }
}
