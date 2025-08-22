using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Errors;
using BaitaHora.Application.Features.Auth.UseCases;
using BaitaHora.Application.Features.Companies.UseCase;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Ports;
using BaitaHora.Application.Services;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Persistence;
using BaitaHora.Infrastructure.Repositories.Auth;
using BaitaHora.Infrastructure.Repositories.Companies;
using BaitaHora.Infrastructure.Repositories.Users;
using BaitaHora.Infrastructure.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class AuthInfrastructureRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
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

        services.AddScoped<ICompanyPermissionService, CompanyPermissionService>();

        // Use Cases
        services.AddScoped<AuthenticateUseCase>();
        services.AddScoped<RegisterOwnerWithCompanyUseCase>();
        services.AddScoped<RegisterEmployeeUseCase>();
        services.AddScoped<RegisterCompanyPositionUseCase>();

        // Repositories
        services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyPositionRepository, CompanyPositionRepository>();


        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddSingleton<IDbErrorTranslator, PostgresDbErrorTranslator>();

        return services;
    }
}