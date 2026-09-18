using API.Middlewares;
namespace API.SystemBuild
{
    public static class ApplicationPipeline
    {
        public static IApplicationBuilder UseApplicationPipeline(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElicTrace");
                c.DocumentTitle = "ElicTrace";
            });

            // على IIS بدون شهادة HTTPS، التحويل الإجباري بيرجّع 307 لعنوان https مسكّر فتفشل طلبات الواجهة
            // وتظهر كأنها مشكلة CORS. فمنخليه اختياري من appsettings.
            if (app.Configuration.GetValue<bool>("UseHttpsRedirection"))
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
