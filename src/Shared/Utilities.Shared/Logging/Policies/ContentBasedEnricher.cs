using Microsoft.AspNetCore.Http;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Logging.Policies;

/// <summary>
/// Sample enricher that adds RequestId, UserId (if available) and environment information.
/// Implement additional enrichers for custom metadata (tenant, trace tags, features).
/// </summary>
public class ContentBasedEnricher : IEnricher
{
    private readonly string _serviceName;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public ContentBasedEnricher(string serviceName, IHttpContextAccessor? httpContextAccessor = null)
    {
        _serviceName = serviceName;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task EnrichRequestAsync(HttpContext context, RequestLogModel model)
    {
        model.Additional["service"] = _serviceName;
        
        // copy correlation/request id if present in header
        if (context.TraceIdentifier is { })
            model.TraceId = context.TraceIdentifier;
        
        // optionally add user id if authenticated
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value
                         ?? context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (userId != null)
                model.Additional["user.id"] = userId;
        }
        
        // example: add environment name from config (via service name param). Could be DIed.
        model.Additional["env"] = _serviceName;
        return Task.CompletedTask;
    }

    public Task EnrichResponseAsync(HttpContext context, ResponseLogModel model)
    {
        // copy trace id from context
        model.TraceId = context.TraceIdentifier;
        model.Additional["service"] = _serviceName;
        return Task.CompletedTask;
    }

    public Task EnrichRequestAsync(HttpRequestMessage request, RequestLogModel model)
    {
        model.Additional["service"] = _serviceName;
        // prefer existing trace header
        if (request.Headers.TryGetValues("X-Request-Id", out var vals))
            model.TraceId = string.Join(",", vals);
        return Task.CompletedTask;
    }

    public Task EnrichResponseAsync(HttpResponseMessage response, ResponseLogModel model)
    {
        model.Additional["service"] = _serviceName;
        return Task.CompletedTask;
    }
}
