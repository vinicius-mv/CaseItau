using CaseItau.Application.Abstractions.Data;
using CaseItau.Infra.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaseItau.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Database") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddSingleton<ISqlConnectionFactory>(sp => new SqliteConnectionFactory(connectionString));

        return services;
    }
}
