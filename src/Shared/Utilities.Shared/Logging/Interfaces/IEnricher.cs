using Microsoft.AspNetCore.Http;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Logging.Interfaces;

/// <summary>
/// Adds structured fields to log contexts for requests/responses.
/// Enrichers should be side-effect free and idempotent.
/// </summary>
public interface IEnricher
{
    /// <summary>
    /// Enrich inbound ASP.NET HttpContext request info (adds properties to request log model).
    /// </summary>
    Task EnrichRequestAsync(HttpContext context, RequestLogModel model);

    /// <summary>
    /// Enrich inbound ASP.NET HttpContext response info (adds properties to response log model).
    /// </summary>
    Task EnrichResponseAsync(HttpContext context, ResponseLogModel model);

    /// <summary>
    /// Enrich outbound HttpClient request.
    /// </summary>
    Task EnrichRequestAsync(HttpRequestMessage request, RequestLogModel model);

    /// <summary>
    /// Enrich outbound HttpClient response.
    /// </summary>
    Task EnrichResponseAsync(HttpResponseMessage response, ResponseLogModel model);
}
