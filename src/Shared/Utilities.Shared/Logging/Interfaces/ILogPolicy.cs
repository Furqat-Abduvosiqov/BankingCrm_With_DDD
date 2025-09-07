using Microsoft.AspNetCore.Http;

namespace Utilities.Shared.Logging.Interfaces;

/// <summary>
/// Represents a high-level policy combining redaction and enrichment decisions.
/// Implementations can encapsulate multiple rules.
/// </summary>
public interface ILogPolicy
{
    /// <summary>
    /// Should this header be redacted from logs (inbound and outbound)?
    /// </summary>
    bool ShouldRedactHeader(string headerName);

    /// <summary>
    /// Should the request body be suppressed from logs (inbound)?
    /// </summary>
    bool ShouldSuppressRequestBody(HttpContext context);

    /// <summary>
    /// Should the request body be suppressed from logs (outbound)?
    /// </summary>
    bool ShouldSuppressRequestBody(HttpRequestMessage request);

    /// <summary>
    /// Should the response body be suppressed from logs (inbound)?
    /// </summary>
    bool ShouldSuppressResponseBody(HttpContext context);

    /// <summary>
    /// Should the response body be suppressed from logs (outbound)?
    /// </summary>
    bool ShouldSuppressResponseBody(HttpResponseMessage response);
}
