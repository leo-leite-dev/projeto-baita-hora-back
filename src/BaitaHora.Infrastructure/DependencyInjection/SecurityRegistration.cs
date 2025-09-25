using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Infrastructure.Web.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class SecurityRegistration
{
    public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ICurrentCompany, HttpContextCurrentCompany>();

        return services;
    }
}