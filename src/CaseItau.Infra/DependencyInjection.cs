using CaseItau.Application.Abstractions.Data;
using CaseItau.Application.Fundos;
using CaseItau.Domain.Abstractions;
using CaseItau.Domain.Fundos;
using CaseItau.Infra.Data;
using CaseItau.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaseItau.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration config)
    {
        // Database
        var connectionString = config.GetConnectionString("Database") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddSingleton<ISqlConnectionFactory>(sp => new SqliteConnectionFactory(connectionString));

        // Entity Framework Core
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Services
        services.AddScoped<IFundoRepository, FundoRepository>();
        services.AddScoped<IFundoReadRepository, FundoReadRepository>();

        return services;
    }
}
