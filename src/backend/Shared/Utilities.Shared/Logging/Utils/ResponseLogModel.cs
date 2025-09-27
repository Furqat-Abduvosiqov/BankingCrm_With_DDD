namespace Utilities.Shared.Logging.Utils;

/// <summary>
/// Lightweight dto model for response logging.
/// </summary>
public sealed class ResponseLogModel
{
    public string TraceId { get; set; } = default!;
    public int StatusCode { get; set; }
    public IDictionary<string, string?> Headers { get; set; } = new Dictionary<string, string?>();
    public string? Body { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public IDictionary<string, object?> Additional { get; } = new Dictionary<string, object?>();
}
