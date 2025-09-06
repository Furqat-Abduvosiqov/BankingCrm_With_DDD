using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Utilities.Shared.Loggers;

public static class LoggingExtensions
{
    /// <summary>
    /// Adds Serilog with optional OpenTelemetry tracing.
    /// Should be called in Program.cs at the very beginning.
    /// </summary>
    public static IHostBuilder UseSerilogWithTelemetry(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, loggerConfig) =>
        {
            var configuration = context.Configuration;

            loggerConfig
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console();

            // Example sinks: Seq / File / ElasticSearch
            var seqUrl = configuration["Logging:SeqUrl"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfig.WriteTo.Seq(seqUrl);
            }

            var filePath = configuration["Logging:FilePath"];
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                loggerConfig.WriteTo.File(
                    filePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);
            }
        });

        return hostBuilder;
    }
    
    /// <summary>
    /// Adds OpenTelemetry tracing to capture request traces across services.
    /// </summary>
    public static IServiceCollection AddTelemetryTracing(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;

                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                            activity.SetTag("requestContentLength", httpRequest.ContentLength);
                        };

                        options.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(serviceName);

                // Exporters
                var otlpEndpoint = configuration["Telemetry:OtlpEndpoint"];
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    builder.AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });

        return services;
    }
}