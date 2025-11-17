using BaitaHora.Application.Abstractions.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Services.Auth.Cookies;
using BaitaHora.Infrastructure.Services.Auth.Security;
using BaitaHora.Infrastructure.Services.Auth.Jwt;
using BaitaHora.Infrastructure.Services.Auth;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class AuthRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TokenOptions>(config.GetSection("JwtOptions"));

        services.Configure<JwtCookieOptions>(config.GetSection("JwtCookieOptions"));
        services.AddScoped<IJwtCookieFactory, JwtCookieFactory>();

        services.AddScoped<IPasswordService, BCryptPasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}