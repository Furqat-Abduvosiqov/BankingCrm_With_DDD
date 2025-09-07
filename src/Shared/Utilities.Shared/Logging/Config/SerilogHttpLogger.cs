using Serilog;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Logging.Config;

/// <summary>
/// Implementation of IHttpLogger that writes structured logs via Serilog.
/// This is the adapter layer between domain-friendly interfaces and Serilog.
/// </summary>
public class SerilogHttpLogger : IHttpLogger
{
    private readonly ILogger _logger;

    public SerilogHttpLogger()
    {
        _logger = Log.ForContext<SerilogHttpLogger>();
    }
    
    public Task LogRequestAsync(RequestLogModel request)
    {
        _logger
            .ForContext("Method", request.Method)
            .ForContext("Url", request.Path)
            .ForContext("Headers", request.Headers, destructureObjects: true)
            .Information("HTTP Request {Method} {Url}", request.Method, request.Path);

        if (!string.IsNullOrEmpty(request.Body))
        {
            _logger.Debug("HTTP Request Body: {Body}", request.Body);
        }
        
        return Task.CompletedTask;
    }

    public Task LogResponseAsync(ResponseLogModel response)
    {
        _logger
            .ForContext("StatusCode", response.StatusCode)
            .ForContext("Headers", response.Headers, destructureObjects: true)
            .Information("HTTP Response {StatusCode}", response.StatusCode);

        if (!string.IsNullOrEmpty(response.Body))
        {
            _logger.Debug("HTTP Response Body: {Body}", response.Body);
        }
        
        return Task.CompletedTask;
    }
}
