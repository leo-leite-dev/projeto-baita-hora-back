using BaitaHora.Application.Common.Caching;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Infrastructure.Services.Companies;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class CompanyRegistration
{
    public static IServiceCollection AddCompanyInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<PermissionCache>();
        services.AddScoped<IUserGuards, UserGuards>();
        services.AddScoped<ICompanyPermissionService, CompanyPermissionService>();
        services.AddScoped<ICompanyGuards, CompanyGuards>();
        services.AddScoped<ICompanyPositionGuards, CompanyPositionGuards>();
        services.AddScoped<ICompanyMemberGuards, CompanyMemberGuards>();
        services.AddScoped<ICompanyServiceOfferingGuards, CompanyServiceOfferingGuards>();

        return services;
    }
}