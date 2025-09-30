// WebApi/Middleware/GlobalExceptionMiddleware.cs
using System.Net;
using System.Text.Json;

namespace CinemaApp.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public GlobalExceptionMiddleware(RequestDelegate next) => _next = next;

    // global exception handling – tüm hataları tek noktada yakalıyorum
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var payload = new
            {
                Title = "Beklenmeyen bir hata oluştu.",
                Status = ctx.Response.StatusCode,
                TraceId = ctx.TraceIdentifier,
                Error = ex.Message // prod'da detayları log'larım, dışarı vermem
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
