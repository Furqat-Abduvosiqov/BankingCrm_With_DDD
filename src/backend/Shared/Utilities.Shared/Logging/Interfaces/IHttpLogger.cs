using Utilities.Shared.Logging.Utils;

namespace Utilities.Shared.Logging.Interfaces;

/// <summary>
/// Adapter interface for structured logging of HTTP requests/responses.
/// Concrete adapters (Serilog, ILogger adapter) implement this.
/// </summary>
public interface IHttpLogger
{
    Task LogRequestAsync(RequestLogModel model);
    Task LogResponseAsync(ResponseLogModel model);
}
