using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Application.Common.Behaviors;

namespace BaitaHora.Application.Configuration;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DbExceptionMappingBehavior<,>));

        return services;
    }
}