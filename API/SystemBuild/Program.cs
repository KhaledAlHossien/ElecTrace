using API.SystemBuild;
using Application;
using Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. تسجيل الخدمات
builder.Services.AddApiRegistrationServices(builder.Configuration);

// 2. إعداد سياسة CORS بشكل صحيح (يجب تحديد الـ Origin عند استخدام AllowCredentials)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.AllowAnyOrigin() // رابط الـ UI الخاص بك
                                //  policy.WithOrigins(builder.Configuration["UIBaseUrl"])
        .AllowAnyHeader()
             .AllowAnyMethod();
            //  .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// 3. تفعيل الـ CORS كأول خطوة في الـ Pipeline
app.UseCors("AllowBlazor");

// 4. باقي الـ Middlewares
app.UseApplicationPipeline();

// تهيئة قاعدة البيانات
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await DbSeeder.SeedAsync(db);
}

// 5. مسارات الـ API والـ Endpoints
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // يفضل تعطيله أثناء التطوير إذا كنت تواجه مشاكل في الشهادات
app.UseAuthorization();
app.MapControllers();

app.Run();