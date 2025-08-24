using BaitaHora.Application.Ports;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class WebAdaptersRegistration
{
    public static IServiceCollection AddWebAdapters(this IServiceCollection services)
    {
        services.AddScoped<IUserIdentityPort, HttpContextUserIdentityAdapter>();
       
        return services;
    }
}