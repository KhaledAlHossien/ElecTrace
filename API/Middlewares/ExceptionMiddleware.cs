using System.Net;
using System.Text.Json;
using FluentValidation;
namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex) // هنا نمسك أخطاء الفاليديشن تحديداً
            {
                await HandleValidationExceptionAsync(httpContext, ex);
            }
            catch (KeyNotFoundException ex) // إمساك خطأ "العنصر غير موجود"
            {
                await HandleKeyNotFoundExceptionAsync(httpContext, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleUnauthorizedExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = exception.Errors
                .Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });

            var result = JsonSerializer.Serialize(new { errors });
            return context.Response.WriteAsync(result);
        }
        private static Task HandleKeyNotFoundExceptionAsync(HttpContext context, KeyNotFoundException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var result = JsonSerializer.Serialize(new { message = exception.Message });
            return context.Response.WriteAsync(result);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new { message = exception.Message });
            return context.Response.WriteAsync(result);
        }

        private static Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var result = JsonSerializer.Serialize(new { message = exception.Message });
            return context.Response.WriteAsync(result);
        }

    }
}
