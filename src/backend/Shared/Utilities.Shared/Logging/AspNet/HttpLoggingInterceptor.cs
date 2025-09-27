using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Logging.AspNet;

/// <summary>
/// A single, configurable interceptor for ASP.NET HttpLogging.
/// It applies configured ILogPolicy/IRedactor/IEnricher rules and uses IHttpLogger to emit structured logs.
/// </summary>
public class HttpLoggingInterceptor : IHttpLoggingInterceptor
{
    private readonly IEnumerable<IRedactor> _redactors;
    private readonly IEnumerable<IEnricher> _enrichers;
    private readonly IHttpLogger _logger;

    public HttpLoggingInterceptor(IEnumerable<IRedactor> redactors, IEnumerable<IEnricher> enrichers, IHttpLogger logger)
    {
        _redactors = redactors;
        _enrichers = enrichers;
        _logger = logger;
    }

    private bool ShouldRedactHeader(string headerName) => _redactors.Any(r => r.ShouldRedactHeader(headerName));
    
    private bool ShouldSuppressRequestBody(HttpContext ctx)
    {
        var ct = ctx.Request.ContentType ?? string.Empty;
        return _redactors.Any(r => r.ShouldSuppressRequestBody(ct, ctx.Request.Method));
    }
    
    private bool ShouldSuppressResponseBody(HttpContext ctx)
    {
        var ct = ctx.Response.ContentType ?? string.Empty;
        return _redactors.Any(r => r.ShouldSuppressResponseBody(ct, ctx.Response.StatusCode));
    }

    public async ValueTask OnRequestAsync(HttpLoggingInterceptorContext context)
    {
        var httpContext = context.HttpContext;
        var model = new RequestLogModel
        {
            Method = httpContext.Request.Method,
            Scheme = httpContext.Request.Scheme,
            Host = httpContext.Request.Host.Value,
            Path = httpContext.Request.Path,
            QueryString = httpContext.Request.QueryString.Value ?? string.Empty,
            // headers
            Headers = HeaderFilters.FilterAndMaskHeaders(httpContext.Request.Headers.Select(
                    h => new KeyValuePair<string, IEnumerable<string>>(h.Key, h.Value)), ShouldRedactHeader)
                    .ToDictionary(k => k.Key, v => v.Value)
        };

       
        if (!ShouldSuppressRequestBody(httpContext))
        {
            // ensure buffering so that other middleware can still read body
            httpContext.Request.EnableBuffering();
            using var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            httpContext.Request.Body.Position = 0;
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            httpContext.Request.Body.Position = 0;
            model.Body = body.Length <= 4096 ? body : body.Substring(0, 4096) + "...";
        }
        else
        {
            model.Body = "[SUPPRESSED]";
        }
        
        foreach (var enr in _enrichers)
        {
            await enr.EnrichRequestAsync(httpContext, model).ConfigureAwait(false);
        }
        
        await _logger.LogRequestAsync(model).ConfigureAwait(false);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public async ValueTask OnResponseAsync(HttpLoggingInterceptorContext context)
    {
        var httpContext = context.HttpContext;
        
        var model = new ResponseLogModel
        {
            StatusCode = httpContext.Response?.StatusCode ?? 0
        };
        
        if (httpContext.Response?.Headers != null)
        {
            model.Headers = HeaderFilters.FilterAndMaskHeaders(httpContext.Response.Headers.Select(
                    h => new KeyValuePair<string, IEnumerable<string>>(h.Key, h.Value)), ShouldRedactHeader)
                .ToDictionary(k => k.Key, v => v.Value);
        }
        
        if (!ShouldSuppressResponseBody(httpContext))
        {
            // Attempt to read response body if response body buffering middleware is used.
            // Here we set a default placeholder to avoid exceptions.
            model.Body = "[RESPONSE_BODY_CAPTURE_REQUIRES_BUFFERING_MIDDLEWARE]";
        }
        else
        {
            model.Body = "[SUPPRESSED]";
        }

        foreach (var enr in _enrichers)
        {
            await enr.EnrichResponseAsync(httpContext, model).ConfigureAwait(false);
        }

        await _logger.LogResponseAsync(model).ConfigureAwait(false);
    }
}