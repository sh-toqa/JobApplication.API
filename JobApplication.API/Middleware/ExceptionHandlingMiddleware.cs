using JobApplication.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace JobApplication.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (ex is NotFoundException or ForbiddenException or BusinessRuleException or UnauthorizedException)
            {
                var statusCode = ex switch
                {
                    NotFoundException => HttpStatusCode.NotFound,
                    ForbiddenException => HttpStatusCode.Forbidden,
                    BusinessRuleException => HttpStatusCode.BadRequest,
                    UnauthorizedException => HttpStatusCode.Unauthorized,
                    _ => HttpStatusCode.InternalServerError
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = ex.Message }));
            }
        }
    }
}
