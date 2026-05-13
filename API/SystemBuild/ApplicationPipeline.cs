using API.Middlewares;
namespace API.SystemBuild
{
    public static class ApplicationPipeline
    {
        public static IApplicationBuilder UseApplicationPipeline(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors("AllowBlazor");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElicTrace");
                c.DocumentTitle = "ElicTrace";
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();




            return app;
        }
    }
}
