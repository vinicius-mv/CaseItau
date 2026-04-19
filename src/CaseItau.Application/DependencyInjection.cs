using CaseItau.Application.Abstractions.Behaviors;
using CaseItau.Application.Fundos.AdicionarFundo;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CaseItau.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.DependencyInjection).Assembly);

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
