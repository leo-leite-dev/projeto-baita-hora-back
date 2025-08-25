using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Infrastructure.Repositories.Auth;
using BaitaHora.Infrastructure.Repositories.Users;
using BaitaHora.Infrastructure.Repositories.Companies;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.Features.Schedules;
using BaitaHora.Infrastructure.Data.Repositories.Schedulings;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyPositionRepository, CompanyPositionRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
       
        return services;
    }
}