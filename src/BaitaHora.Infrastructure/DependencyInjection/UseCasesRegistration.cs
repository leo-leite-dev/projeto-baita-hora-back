using BaitaHora.Application.Features.Auth.UseCases;
using BaitaHora.Application.Features.Companies.UseCases;
using BaitaHora.Application.Features.Users.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class UseCasesRegistration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<AuthenticateUseCase>();
        services.AddScoped<RegisterOwnerWithCompanyUseCase>();
        services.AddScoped<RegisterEmployeeUseCase>();
        services.AddScoped<CreateCompanyPositionUseCase>();
        services.AddScoped<CreateCompanyServiceUseCase>();
        services.AddScoped<PatchEmployeeUseCase>();
        services.AddScoped<ToggleUserActiveUseCase>();
        return services;
    }
}