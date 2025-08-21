using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Ports;
using BaitaHora.Application.Auth.UseCases.Authenticate;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Services.Auth;
using BaitaHora.Infrastructure.Repositories.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Infrastructure.Repositories;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.Common;
using BaitaHora.Infrastructure.Persistence;
using BaitaHora.Infrastructure.Services;

namespace BaitaHora.Infrastructure.Extensions;

public static class AuthInfrastructureRegistration
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // JWT Options
        services.Configure<TokenOptions>(config.GetSection("JwtOptions"));

        // Services
        services.AddScoped<IPasswordService, BCryptPasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ICookieService, CookieService>();

        // Ports
        services.AddScoped<IUserIdentityPort, HttpContextUserIdentityAdapter>();

        // Use Cases
        services.AddScoped<AuthenticateUseCase>();
        services.AddScoped<RegisterOwnerWithCompanyUseCase>();

        // Repositories
        services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}
