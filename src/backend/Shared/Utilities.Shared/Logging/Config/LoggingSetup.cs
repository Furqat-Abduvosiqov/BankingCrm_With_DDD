using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Utilities.Shared.Logging.AspNet;
using Utilities.Shared.Logging.HttpClient;
using Utilities.Shared.Logging.Interfaces;
using Utilities.Shared.Logging.Policies;

namespace Utilities.Shared.Logging.Config;

/// <summary>
/// Composition root helper: registers logging, default policies and the interceptor/handler.
/// Call LoggingSetup.RegisterLoggingServices(builder.Services, configuration...) from Program.cs.
/// </summary>
public static class LoggingSetup
{
    
    /// <summary>
    /// Adds Serilog (Console + Seq) as the structured logger.
    /// </summary>
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Seq(configuration["Logging:SeqUrl"] ?? "http://localhost:5341")
            .CreateLogger();

        services.AddSingleton<IHttpLogger, SerilogHttpLogger>();

        return services;
    }

    /// <summary>
    /// Adds OpenTelemetry tracing + metrics + logging correlation.
    /// </summary>
    public static IServiceCollection AddOpenTelemetryLogging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(configuration["ServiceName"] ?? "MyApp"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource("Logging")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Telemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Telemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                }));
        
        services.AddLogging(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
                logging.ParseStateValues = true;
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Telemetry:OtlpEndpoint"] ?? "http://localhost:4317");
                });
            });
        });

        return services;
    }
    
    /// <summary>
    /// Adds ASP.NET + HttpClient logging with redaction and enrichment policies.
    /// </summary>
    public static IServiceCollection AddHttpLoggingWithPolicies(this IServiceCollection services, string serviceName = "")
    {
        // Core: redactor/policy implementations
        services.AddSingleton<IRedactor, DefaultRedactionPolicy>();
        services.AddSingleton<ILogPolicy, DefaultRedactionPolicy>();

        // Enricher(s) - example: register the content-based enricher
        services.AddSingleton<IEnricher>(sp =>
            new ContentBasedEnricher(serviceName, sp.GetService<IHttpContextAccessor>()));

        // ASP.NET built-in HttpLogging
        services.AddHttpLogging(HttpLoggingOptionsFactory.ConfigureDefaults);

        // Add interceptor for custom policies
        services.AddSingleton<IHttpLoggingInterceptor, HttpLoggingInterceptor>();

        // HttpClient delegating handler
        services.AddTransient<LoggingDelegatingHandler>();

        // IHttpContextAccessor is needed for enrichers
        services.AddHttpContextAccessor();

        return services;
    }
}
