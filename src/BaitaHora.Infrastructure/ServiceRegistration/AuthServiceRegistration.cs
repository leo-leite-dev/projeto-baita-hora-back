using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Caching;
using BaitaHora.Application.Common.Errors;
using BaitaHora.Application.Common.Interfaces;
using BaitaHora.Application.Features.Auth.UseCases;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.UseCase;
using BaitaHora.Application.Features.Users.UseCases;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Ports;
using BaitaHora.Infrastructure.Configuration;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Data.Outbox;
using BaitaHora.Infrastructure.Repositories;
using BaitaHora.Infrastructure.Repositories.Auth;
using BaitaHora.Infrastructure.Repositories.Companies;
using BaitaHora.Infrastructure.Repositories.Users;
using BaitaHora.Infrastructure.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure;

public static class AuthServiceRegistration
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // -------------------------------a
        // Options
        // -------------------------------
        services.Configure<TokenOptions>(config.GetSection("JwtOptions"));
        services.Configure<WhatsAppOptions>(config.GetSection("WhatsApp"));

        // -------------------------------
        // Infra services (Auth)
        // -------------------------------
        services.AddScoped<IPasswordService, BCryptPasswordService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<ICookieService, CookieService>();

        // -------------------------------
        // Ports / Adapters
        // -------------------------------
        services.AddScoped<IUserIdentityPort, HttpContextUserIdentityAdapter>();

        // -------------------------------
        // Permission cache & permission service
        // -------------------------------
        services.AddSingleton<PermissionCache>();
        services.AddScoped<ICompanyPermissionService, CompanyPermissionService>();

        // -------------------------------
        // Guards (TODOS os usados nos UseCases)
        // -------------------------------
        services.AddScoped<ICompanyGuards, CompanyGuards>();
        services.AddScoped<ICompanyPositionGuards, CompanyPositionGuards>(); // <- faltava
        services.AddScoped<ICompanyMemberGuards, CompanyMemberGuards>();     // <- faltava

        // -------------------------------
        // Use Cases usados pelos Handlers
        // -------------------------------
        services.AddScoped<AuthenticateUseCase>();
        services.AddScoped<RegisterOwnerWithCompanyUseCase>();
        services.AddScoped<RegisterEmployeeUseCase>();
        services.AddScoped<RegisterCompanyPositionUseCase>();
        services.AddScoped<PatchEmployeeUseCase>();
        services.AddScoped<ToggleUserActiveUseCase>();

        // -------------------------------
        // Repositories
        // -------------------------------
        // Open generic para resolver IGenericRepository<User> (e quaisquer outros)
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); // <- faltava

        // Específicos
        services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyPositionRepository, CompanyPositionRepository>();

        // -------------------------------
        // UoW
        // -------------------------------
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // -------------------------------
        // DB Error Translator
        // -------------------------------
        services.AddSingleton<IDbErrorTranslator, PostgresDbErrorTranslator>();

        // -------------------------------
        // Outbox / Messaging
        // -------------------------------
        services.AddScoped<IOutboxPublisher, OutboxPublisher>();
        services.AddHttpClient<WhatsAppBus>();
        services.AddScoped<IBus, WhatsAppBus>();

        return services;
    }
}