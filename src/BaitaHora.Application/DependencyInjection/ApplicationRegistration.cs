using BaitaHora.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaitaHora.Application.DependencyInjection;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationRegistration).Assembly);

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
            cfg.AddOpenBehavior(typeof(DomainEventsBehavior<,>));
            cfg.AddOpenBehavior(typeof(IntegrationEventsBehavior<,>));
            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));

            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));   // opcional
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));  // 🚀 necessário
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>)); // opcional
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}