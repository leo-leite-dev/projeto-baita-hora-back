using BaitaHora.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Application.DependencyInjection;

public static class PipelineRegistration
{
    public static IServiceCollection AddApplicationPipelines(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));     // pré-commit
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehavior<,>));       // commit
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IntegrationEventsBehavior<,>));// pós-commit

        return services;
    }
}