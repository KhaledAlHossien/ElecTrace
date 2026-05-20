using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;
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
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:54982/";
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<CustomAuthorizationHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("APIClient"));

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();