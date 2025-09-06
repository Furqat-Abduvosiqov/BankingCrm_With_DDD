using Microsoft.AspNetCore.Builder;

namespace Utilities.Shared.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseApiAccessLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiAccessLoggingMiddleware>();
    }
}