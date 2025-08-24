using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Application.DependencyInjection;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistration).Assembly));

        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

        services.AddApplicationPipelines();

        return services;
    }
}