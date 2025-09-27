using Microsoft.AspNetCore.Http;
using Utilities.Shared.Logging.Interfaces;

namespace Utilities.Shared.Logging.Policies;

/// <summary>
/// Default redaction rules: a small list of sensitive headers and body suppression heuristics.
/// </summary>
public class DefaultRedactionPolicy : IRedactor, ILogPolicy
{
    private static readonly string[] DefaultSensitiveHeaders =
    [
        "Authorization",
        "Proxy-Authorization",
        "X-Api-Key",
        "X-API-Key",
        "Set-Cookie",
        "Cookie"
    ];

    private readonly string[] _sensitiveHeaders;

    public DefaultRedactionPolicy(string[]? additionalSensitiveHeaders = null)
    {
        _sensitiveHeaders = (DefaultSensitiveHeaders
            .Concat(additionalSensitiveHeaders ?? Array.Empty<string>()))
            .ToArray();
    }

    // IRedactor
    public bool ShouldRedactHeader(string headerName)
        => _sensitiveHeaders.Any(h => headerName.Contains(h, StringComparison.OrdinalIgnoreCase));

    public bool ShouldSuppressRequestBody(string contentType, string httpMethod)
    {
        if (string.IsNullOrEmpty(contentType)) return false;
        
        // Binary or file uploads: suppress body for logs
        if (contentType.Contains("application/octet-stream") || contentType.Contains("multipart/form-data"))
            return true;
        
        // Optionally suppress for long/unsafe methods
        if (httpMethod.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    }

    public bool ShouldSuppressResponseBody(string contentType, int statusCode)
    {
        if (string.IsNullOrEmpty(contentType)) return false;
        if (contentType.Contains("application/octet-stream")) return true;
        if (contentType.Contains("multipart/form-data")) return true;
        
        // On 5xx maybe suppress or only log small partial content — here we choose to keep it
        return false;
    }
    
    #region ILogPolicy Members
    
        bool ILogPolicy.ShouldRedactHeader(string headerName) => ShouldRedactHeader(headerName);

        bool ILogPolicy.ShouldSuppressRequestBody(HttpContext context)
        {
            var ct = context.Request.ContentType ?? string.Empty;
            return ShouldSuppressRequestBody(ct, context.Request.Method);
        }

        bool ILogPolicy.ShouldSuppressRequestBody(HttpRequestMessage request)
        {
            var ct = request.Content?.Headers.ContentType?.MediaType ?? string.Empty;
            return ShouldSuppressRequestBody(ct, request.Method.Method);
        }

        bool ILogPolicy.ShouldSuppressResponseBody(HttpContext context)
        {
            var ct = context.Response.ContentType ?? string.Empty;
            return ShouldSuppressResponseBody(ct, context.Response.StatusCode);
        }

        bool ILogPolicy.ShouldSuppressResponseBody(HttpResponseMessage response)
        {
            var ct = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            return ShouldSuppressResponseBody(ct, (int)response.StatusCode);
        }
    
    #endregion
}
    