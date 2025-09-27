using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Utilities.Shared.ApiVersioning;

public static class SwaggerOptionsExtensions
{
    public static void AddVersionedSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }

    public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "API",
                        Version = description.ApiVersion.ToString(),
                        Description = description.IsDeprecated
                            ? $"This API version {description.ApiVersion} has been deprecated."
                            : $"API Version {description.ApiVersion}"
                    });
            }
        }

        public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
    }
}