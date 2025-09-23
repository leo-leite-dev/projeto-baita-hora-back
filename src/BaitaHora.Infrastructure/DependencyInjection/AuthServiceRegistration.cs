using BaitaHora.Application.Abstractions.Auth;          // <- novo
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Services.Auth;
using BaitaHora.Infrastructure.Services.Auth.Cookies;
using BaitaHora.Infrastructure.Services.Auth.Jwt;
using BaitaHora.Infrastructure.Services.Auth.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class AuthRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // JWT
        services.Configure<TokenOptions>(config.GetSection("JwtOptions"));

        // Cookie (se estiver usando)
        services.Configure<JwtCookieOptions>(config.GetSection("JwtCookieOptions"));
        services.AddScoped<IJwtCookieFactory, JwtCookieFactory>();

        // Serviços de Auth
        services.AddScoped<IPasswordService, BCryptPasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>(); 

        return services;
    }
}