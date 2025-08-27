using BaitaHora.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation; 


namespace BaitaHora.Application.DependencyInjection;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistration).Assembly));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PersistenceExceptionMappingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IntegrationEventsBehavior<,>));

        return services;
    }
}