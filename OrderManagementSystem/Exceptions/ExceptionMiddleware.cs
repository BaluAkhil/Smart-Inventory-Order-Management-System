using System.Net;
using System.Text.Json;

namespace OrderManagementSystem.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                if (ex is AppException appException)
                {
                    _logger.LogWarning(
                        "{ExceptionType} while processing {Method} {Path}: {Message}",
                        ex.GetType().Name, context.Request.Method, context.Request.Path, ex.Message);

                    context.Response.StatusCode = appException.StatusCode;

                    var response = new
                    {
                        statusCode = appException.StatusCode,
                        message = appException.Message,
                        traceId = context.TraceIdentifier
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
                else
                {
                    _logger.LogError(ex,
                        "Unhandled exception while processing {Method} {Path}",
                        context.Request.Method, context.Request.Path);

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var response = new
                    {
                        statusCode = 500,
                        message = "Something went wrong.",
                        traceId = context.TraceIdentifier
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                }
            }
        }
    }
}