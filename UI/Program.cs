using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;
using UI.Services;
using UI.Services.Interface;
using UI.Services.Repo;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// تم حذف builder.Configuration.AddJsonFile لأن التحميل تلقائي
// ولكن يمكنك التأكد من وجود appsettings.json في wwwroot

builder.Services.AddScoped<CustomAuthorizationHandler>();

builder.Services.AddHttpClient("APIClient", client =>
{
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://{host}:7033/";

    // {host} = اسم السيرفر أو الـ IP اللي فتح منه المستخدم الواجهة.
    // هيك نفس النشر بيشتغل من أي جهاز بالشبكة بدون تعديل، بدل ما localhost تأشّر على جهاز المستخدم نفسه.
    apiBaseUrl = apiBaseUrl.Replace("{host}", new Uri(builder.HostEnvironment.BaseAddress).Host);

    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("APIClient"));

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiDataService, ApiDataService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<DialogService>();

await builder.Build().RunAsync();
