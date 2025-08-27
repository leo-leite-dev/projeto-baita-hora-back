using BaitaHora.Application.Ports;

namespace BaitaHora.Api.Web.Adapters;

public static class WebAdaptersRegistration
{
    public static IServiceCollection AddWebAdapters(this IServiceCollection services)
    {
        services.AddScoped<IUserIdentityPort, HttpContextUserIdentityAdapter>();
       
        return services;
    }
}