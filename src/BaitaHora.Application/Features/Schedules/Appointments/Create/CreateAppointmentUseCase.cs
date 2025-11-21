using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Schedules.Appointments.Create;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Application.IRepositories.Schedules;
using BaitaHora.Domain.Features.Schedules.Entities;

public sealed class CreateAppointmentUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly ICompanyServiceOfferingRepository _serviceRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateAppointmentUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany,
        ICompanyMemberRepository companyMemberRepository,
        ICompanyServiceOfferingRepository serviceRepository,
        IScheduleRepository scheduleRepository,
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
        _companyMemberRepository = companyMemberRepository;
        _serviceRepository = serviceRepository;
        _scheduleRepository = scheduleRepository;
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<Guid>> HandleAsync(CreateAppointmentCommand cmd, CancellationToken ct)
    {
        var companyId = _currentCompany.Id;
        if (companyId == Guid.Empty)
            return Result<Guid>.Unauthorized("Empresa atual não identificada.");

        var membership = await _companyMemberRepository.GetByIdAsync(cmd.MemberId, ct);
        if (membership is null || membership.CompanyId != companyId || !membership.IsActive)
            return Result<Guid>.BadRequest("MemberId inválido para a empresa atual.");

        var guard = await _companyGuards.GetActiveMembership(companyId, cmd.MemberId, ct);
        if (guard.IsFailure)
            return Result<Guid>.FromError(guard);

        if (cmd.DurationMinutes <= 0)
            return Result<Guid>.BadRequest("Duração deve ser maior que zero.");

        if (cmd.StartsAtUtc.Kind != DateTimeKind.Utc)
            return Result<Guid>.BadRequest("StartsAtUtc deve estar em UTC.");

        var serviceIds =
            cmd.ServiceOfferingIds?
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray() ?? Array.Empty<Guid>();

        if (serviceIds.Length == 0)
            return Result<Guid>.BadRequest("Selecione ao menos um serviço.");

        var services = await _serviceRepository.GetByIdsAsync(serviceIds, ct);
        if (services.Count != serviceIds.Length)
            return Result<Guid>.BadRequest("Alguns serviços informados não existem.");

        if (services.Any(s => s.CompanyId != companyId))
            return Result<Guid>.BadRequest("Alguns serviços não pertencem à empresa atual.");

        if (services.Any(s => !s.IsActive))
            return Result<Guid>.BadRequest("Há serviços inativos na seleção.");

        var customer = await _customerRepository.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null)
            return Result<Guid>.NotFound("Cliente não encontrado.");

        var schedule = await _scheduleRepository.GetByMemberIdAsync(cmd.MemberId, ct)
                      ?? Schedule.Create(cmd.MemberId);

        if (schedule.Id == Guid.Empty)
            await _scheduleRepository.AddAsync(schedule, ct);

        var duration = TimeSpan.FromMinutes(cmd.DurationMinutes);
        var appointment = schedule.AddAppointment(customer, services, cmd.StartsAtUtc, duration);

        await _appointmentRepository.AddAsync(appointment, ct);

        return Result<Guid>.Created(appointment.Id);
    }
}