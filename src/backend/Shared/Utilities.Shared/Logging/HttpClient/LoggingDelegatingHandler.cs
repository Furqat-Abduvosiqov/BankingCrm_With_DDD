using System.Diagnostics.CodeAnalysis;
using System.Text;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Utils;


namespace Utilities.Shared.Logging.HttpClient;

/// <summary>
/// DelegatingHandler that logs outgoing HttpClient requests/responses using shared policies and enrichers.
/// </summary>
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly IEnumerable<IRedactor> _redactors;
    private readonly IEnumerable<IEnricher> _enrichers;
    private readonly IHttpLogger _logger;

    public LoggingDelegatingHandler(IEnumerable<IRedactor> redactors, IEnumerable<IEnricher> enrichers, IHttpLogger logger)
    {
        _redactors = redactors;
        _enrichers = enrichers;
        _logger = logger;
    }

    private bool ShouldRedactHeader(string headerName) => _redactors.Any(r => r.ShouldRedactHeader(headerName));
    private bool ShouldSuppressRequestBody(HttpRequestMessage request)
    {
        var ct = request.Content?.Headers.ContentType?.MediaType ?? string.Empty;
        return _redactors.Any(r => r.ShouldSuppressRequestBody(ct, request.Method.Method));
    }
    private bool ShouldSuppressResponseBody(HttpResponseMessage response)
    {
        var ct = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
        return _redactors.Any(r => r.ShouldSuppressResponseBody(ct, (int)response.StatusCode));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var reqModel = new RequestLogModel
        {
            Method = request.Method.Method,
            Path = request.RequestUri?.AbsolutePath,
            QueryString = request.RequestUri?.Query,
            Scheme = request.RequestUri?.Scheme,
            Host = request.RequestUri?.Host,
            // headers
            Headers = HeaderFilters.FilterAndMaskHeaders(request.Headers, ShouldRedactHeader)
                .ToDictionary(k => k.Key, v => v.Value)
        };

        // body for request
        if (!ShouldSuppressRequestBody(request) && request.Content != null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            var body = bytes != null && bytes.Length > 0 ? Encoding.UTF8.GetString(bytes) : string.Empty;
            reqModel.Body = body.Length <= 4096 ? body : body.Substring(0, 4096) + "...";
        }
        else
        {
            reqModel.Body = "[SUPPRESSED]";
        }

        foreach (var enr in _enrichers)
            await enr.EnrichRequestAsync(request, reqModel).ConfigureAwait(false);

        await _logger.LogRequestAsync(reqModel).ConfigureAwait(false);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        sw.Stop();

        var respModel = new ResponseLogModel
        {
            StatusCode = (int)response.StatusCode,
            Duration = sw.Elapsed,
            Headers = HeaderFilters.FilterAndMaskHeaders(response.Headers, ShouldRedactHeader)
                .ToDictionary(k => k.Key, v => v.Value)
        };

        if (!ShouldSuppressResponseBody(response) && response.Content is not null)
        {
            // beware: reading content here consumes it — clone if other consumers need it
            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            var body = bytes is not null && bytes.Length > 0 ? Encoding.UTF8.GetString(bytes) : string.Empty;
            respModel.Body = body.Length <= 4096 ? body : body.Substring(0, 4096) + "...";
        }
        else
        {
            respModel.Body = "[SUPPRESSED]";
        }

        foreach (var enr in _enrichers)
            await enr.EnrichResponseAsync(response, respModel).ConfigureAwait(false);

        await _logger.LogResponseAsync(respModel).ConfigureAwait(false);

        return response;
    }
}