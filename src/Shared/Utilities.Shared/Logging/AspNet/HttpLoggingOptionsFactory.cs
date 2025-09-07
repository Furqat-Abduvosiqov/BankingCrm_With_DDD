using Microsoft.AspNetCore.HttpLogging;

namespace Utilities.Shared.Logging.AspNet;

/// <summary>
/// Factory helper to configure built-in HttpLoggingOptions.
/// Keep this small — interceptor will implement deeper logic.
/// </summary>
public static class HttpLoggingOptionsFactory
{
    public static void ConfigureDefaults(HttpLoggingOptions? options)
    {
        if (options is null) return;
        
        // Choose a baseline set of fields. The interceptor/policies will do redaction/suppression.
        options.LoggingFields =
            HttpLoggingFields.RequestMethod |
            HttpLoggingFields.RequestPath |
            HttpLoggingFields.RequestQuery |
            HttpLoggingFields.RequestHeaders |
            HttpLoggingFields.RequestBody |
            HttpLoggingFields.ResponseStatusCode |
            HttpLoggingFields.ResponseHeaders |
            HttpLoggingFields.ResponseBody;
        
        // Increase request/response size limits if you need body logging
        options.RequestBodyLogLimit = 4096;
        options.ResponseBodyLogLimit = 4096;
    }
}