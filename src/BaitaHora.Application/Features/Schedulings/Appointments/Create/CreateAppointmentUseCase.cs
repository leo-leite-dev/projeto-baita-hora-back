using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed class CreateAppointmentUseCase
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateAppointmentUseCase(
        IScheduleRepository scheduleRepository,
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository)
    {
        _scheduleRepository = scheduleRepository;
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<CreateAppointmentResponse>> HandleAsync(
        CreateAppointmentCommand cmd, CancellationToken ct)
    {
        var schedule = await _scheduleRepository.GetByMemberIdAsync(cmd.MemberId, ct);
        if (schedule is null)
        {
            schedule = Schedule.Create(cmd.MemberId);
            await _scheduleRepository.AddAsync(schedule, ct);
        }

        var customer = await _customerRepository.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null)
            return Result<CreateAppointmentResponse>.NotFound("Cliente não encontrado.");

        if (cmd.DurationMinutes <= 0)
            return Result<CreateAppointmentResponse>.BadRequest("Duração deve ser maior que zero.");

        var duration = TimeSpan.FromMinutes(cmd.DurationMinutes);
        var appt = Appointment.Create(schedule, customer, cmd.StartsAtUtc, duration);

        await _appointmentRepository.AddAsync(appt, ct);

        var response = new CreateAppointmentResponse(
            AppointmentId: appt.Id,
            ScheduleId: schedule.Id,
            CompanyId: cmd.CompanyId,
            MemberId: cmd.MemberId,
            CustomerId: cmd.CustomerId,
            StartsAtUtc: appt.StartsAtUtc,
            EndsAtUtc: appt.EndsAtUtc
        );

        return Result<CreateAppointmentResponse>.Created(response);
    }
}