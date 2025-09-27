using System.Net.Http.Headers;

namespace Utilities.Shared.Logging.Utils;

/// <summary>
/// Small utility helpers to work with headers and apply redaction rules.
/// Keep this pure and easily testable.
/// </summary>
public static class HeaderFilters
{
    /// <summary>
    /// Mask value to use when header is redacted.
    /// </summary>
    public const string Mask = "[REDACTED]";

    public static IReadOnlyDictionary<string, string?> FilterAndMaskHeaders(
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
        Func<string, bool> shouldRedact)
    {
        var dict = new Dictionary<string, string?>();
        foreach (var kv in headers)
        {
            var key = kv.Key;
            if (shouldRedact(key))
            {
                dict[key] = Mask;
            }
            else
            {
                // join multiple values with comma
                dict[key] = string.Join(",", kv.Value ?? Enumerable.Empty<string>());
            }
        }
        return dict;
    }

    public static IReadOnlyDictionary<string, string?> FilterAndMaskHeaders(
        HttpHeaders headers,
        Func<string, bool> shouldRedact)
    {
        var dict = new Dictionary<string, string?>();
        foreach (var header in headers)
        {
            var key = header.Key;
            if (shouldRedact(key))
            {
                dict[key] = Mask;
            }
            else
            {
                dict[key] = string.Join(",", header.Value ?? Enumerable.Empty<string>());
            }
        }
        return dict;
    }
}