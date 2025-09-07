using System.Diagnostics.CodeAnalysis;

namespace Utilities.Shared.Logging.Utils;

/// <summary>
/// Lightweight dto model for request logging.
/// </summary>
[SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
public sealed class RequestLogModel
{
    public string TraceId { get; set; } = null!;
    public string? Method { get; set; }
    public string? Scheme { get; set; }
    public string? Host { get; set; }
    public string? Path { get; set; }
    public string? QueryString { get; set; }
    public Dictionary<string, string?> Headers { get; set; } = new ();
    public string? Body { get; set; }
    
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object?> Additional { get; set; } = new ();
}
