using Microsoft.OpenApi.Models;

namespace CaseItau.API.Extensions;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CaseItau.API",
                Version = "v1",
                Description = "API para controle e cadastro de fundos"
            });

            //options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
        });

        return services;
    }
}
