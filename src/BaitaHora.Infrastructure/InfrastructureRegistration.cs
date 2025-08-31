using BaitaHora.Infrastructure.Data.Behaviors;
using BaitaHora.Infrastructure.Common.Errors;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Infrastructure.DependencyInjection;
using BaitaHora.Application.Common.Errors;

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

        services.AddSingleton<IDbErrorTranslator, PostgresDbErrorTranslator>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EfPersistenceExceptionMappingBehavior<,>));

        return services;
    }
}