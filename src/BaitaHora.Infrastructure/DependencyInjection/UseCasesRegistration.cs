using BaitaHora.Application.Features.Auth;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using BaitaHora.Application.Features.Companies.Catalog.Patch;
using BaitaHora.Application.Features.Companies.Members.Employee;
using BaitaHora.Application.Features.Companies.Members.Employee.Register;
using BaitaHora.Application.Features.Companies.Members.Owner;
using BaitaHora.Application.Features.Companies.Positions.Create;
using BaitaHora.Application.Features.Companies.Positions.Patch;
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
        services.AddScoped<CreateCompanyServiceOfferingUseCase>();
        services.AddScoped<PatchOwnerUseCase>();
        services.AddScoped<PatchEmployeeUseCase>();
        services.AddScoped<PatchCompanyPositionUseCase>();
        services.AddScoped<PatchCompanyServiceOfferingUseCase>();
        services.AddScoped<ToggleUserActiveUseCase>();
        return services;
    }
}