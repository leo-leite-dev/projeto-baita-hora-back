using BaitaHora.Application.IServices.Auth;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class AuthRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TokenOptions>(config.GetSection("JwtOptions"));

        services.AddScoped<IPasswordService, BCryptPasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}