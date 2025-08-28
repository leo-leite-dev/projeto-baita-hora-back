using BaitaHora.Application.Features.Auth;
using BaitaHora.Application.Features.Companies.Members.Employee.Patch;
using BaitaHora.Application.Features.Companies.Members.Employee.Register;
using BaitaHora.Application.Features.Companies.UseCases;
using BaitaHora.Application.Features.Onboarding;
using BaitaHora.Application.Features.Users.CreateUser;
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