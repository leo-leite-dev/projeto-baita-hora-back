using BaitaHora.Application.Features.Onboarding;
using BaitaHora.Application.Features.Companies.Members.Owner;
using BaitaHora.Application.Features.Companies.Members.Employee;
using BaitaHora.Application.Features.Companies.Members.Employee.Register;
using BaitaHora.Application.Features.Companies.Positions.Create;
using BaitaHora.Application.Features.Companies.Positions.Patch;
using BaitaHora.Application.Features.Companies.Positions.Remove;
using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Application.Features.Companies.ServiceOffering.Remove;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using Microsoft.Extensions.DependencyInjection;
using BaitaHora.Application.Features.Companies.Positions.Disable;
using BaitaHora.Application.Features.Companies.Positions.Activate;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;
using BaitaHora.Application.Features.Companies.Employees.Disable;
using BaitaHora.Application.Features.Companies.Employees.Activate;
using BaitaHora.Application.Companies.Features.Members.Promotion;
using BaitaHora.Application.Features.Schedulings.Appointments.Create;
using BaitaHora.Application.Features.Customers.Create;
using BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;
using BaitaHora.Application.Features.Schedulings.Appointments.GetAll;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;
using BaitaHora.Application.Features.Auth.Login;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class UseCasesRegistration
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<AuthenticateUseCase>();
        // services.AddScoped<ToggleUserActiveUseCase>();

        // Customer
        services.AddScoped<CreateCustomerUseCase>();

        // Onboarding
        services.AddScoped<RegisterOwnerWithCompanyUseCase>();
        services.AddScoped<RegisterEmployeeUseCase>();

        // Companies → Members
        services.AddScoped<PatchOwnerUseCase>();
        services.AddScoped<PatchEmployeeUseCase>();
        services.AddScoped<DisableEmployeesUseCase>();
        services.AddScoped<ActivateEmployeesUseCase>();
        services.AddScoped<ChangeMemberPositionUseCase>();

        // Companies → Positions
        services.AddScoped<CreatePositionUseCase>();
        services.AddScoped<PatchPositionUseCase>();
        services.AddScoped<RemovePositionUseCase>();
        services.AddScoped<ActivatePositionsUseCase>();
        services.AddScoped<DisablePositionsUseCase>();

        // Companies → Service Offerings
        services.AddScoped<CreateServiceOfferingUseCase>();
        services.AddScoped<PatchServiceOfferingUseCase>();
        services.AddScoped<RemoveServiceOfferingUseCase>();
        services.AddScoped<ActivateServiceOfferingsUseCase>();
        services.AddScoped<DisableServiceOfferingsUseCase>();

        //Schedulings
        services.AddScoped<CreateAppointmentUseCase>();
        services.AddScoped<RescheduleAppointmentUseCase>();
        services.AddScoped<GetAppointmentsUseCase>();

        return services;
    }
}