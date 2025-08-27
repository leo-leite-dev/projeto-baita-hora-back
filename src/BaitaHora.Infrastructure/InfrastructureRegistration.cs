using BaitaHora.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddInfrastructureCore(config)
            .AddRepositories()
            .AddAuthInfrastructure(config)
            .AddCompanyInfrastructure()
            .AddMessagingInfrastructure(config)
            .AddUseCases()
                .AddBotInfrastructure(config);

        return services;
    }
}