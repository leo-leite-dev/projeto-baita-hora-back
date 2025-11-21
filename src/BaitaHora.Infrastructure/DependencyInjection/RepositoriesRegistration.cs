using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Application.IRepositories.Schedules;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Infrastructure.Repositories.Auth;
using BaitaHora.Infrastructure.Repositories.Companies;
using BaitaHora.Infrastructure.Repositories.Schedules;
using BaitaHora.Infrastructure.Repositories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class RepositoriesRegistration
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<ILoginSessionRepository, LoginSessionRepository>();

        // Users
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        // Companies
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyPositionRepository, CompanyPositionRepository>();
        services.AddScoped<ICompanyServiceOfferingRepository, CompanyServiceOfferingRepository>();
        services.AddScoped<ICompanyStatsReadRepository, CompanyStatsReadRepository>();

        // Scheduling
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        // Customers
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}