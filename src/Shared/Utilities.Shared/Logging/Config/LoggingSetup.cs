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
            .WriteTo.Console()
            .WriteTo.Seq(configuration["Seq:Url"] ?? "http://localhost:5341")
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
                    options.Endpoint = new Uri(configuration["Otlp:Endpoint"] ?? "http://localhost:4317");
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Otlp:Endpoint"] ?? "http://localhost:4317");
                }));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
                logging.ParseStateValues = true;
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["Otlp:Endpoint"] ?? "http://localhost:4317");
                });
            });
        });

        return services;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection RegisterLoggingServices(this IServiceCollection services, string serviceName = "my-service")
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Core: redactor/policy implementations
        services.AddSingleton<IRedactor, DefaultRedactionPolicy>();
        services.AddSingleton<ILogPolicy, DefaultRedactionPolicy>();

        // Enricher(s) - example: register the content-based enricher
        services.AddSingleton<IEnricher>(sp =>
            new ContentBasedEnricher(serviceName, sp.GetService<IHttpContextAccessor>()));

        // Logging adapter - implement IHttpLogger with Serilog adapter or your own
        services.AddSingleton<IHttpLogger, SerilogHttpLogger>();

        // ASP.NET built-in HttpLogging - add options but keep interceptor for deeper logic
        services.AddHttpLogging(options =>
        {
            // Basic fields; interceptor will apply suppression/redaction
            HttpLoggingOptions defaults = options;
            HttpLoggingOptionsFactory.ConfigureDefaults(defaults);
        });

        // Add the interceptor to be used by AddHttpLogging
        services.AddSingleton<IHttpLoggingInterceptor, HttpLoggingInterceptor>();

        // register the HttpClient delegating handler
        services.AddTransient<LoggingDelegatingHandler>();

        // It's often useful to add IHttpContextAccessor for enrichers
        services.AddHttpContextAccessor();

        return services;
    }
}
