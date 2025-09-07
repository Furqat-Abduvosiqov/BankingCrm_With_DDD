namespace Utilities.Shared.Logging.Interfaces;

/// <summary>
/// A single-purpose policy that knows how to redact things (headers / bodies or other sensitive fields).
/// Use multiple implementations composed via DI if necessary.
/// </summary>
public interface IRedactor
{
    bool ShouldRedactHeader(string headerName);
    bool ShouldSuppressRequestBody(string contentType, string httpMethod);
    bool ShouldSuppressResponseBody(string contentType, int statusCode);
}