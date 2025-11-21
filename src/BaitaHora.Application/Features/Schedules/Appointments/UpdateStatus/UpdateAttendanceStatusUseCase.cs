using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Application.IRepositories.Schedules;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Schedules.Enums;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.UpdateStatus;

public sealed class UpdateAttendanceStatusUseCase
{
    private readonly IScheduleGuards _scheduleGuards;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICustomerRepository _customerRepository;

    public UpdateAttendanceStatusUseCase(
        IScheduleGuards scheduleGuards,
        IAppointmentRepository appointmentRepository,
        ICustomerRepository customerRepository)
    {
        _scheduleGuards = scheduleGuards;
        _appointmentRepository = appointmentRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<Unit>> HandleAsync(
        UpdateAttendanceStatusCommand cmd,
        CancellationToken ct)
    {
        var guardRes = await _scheduleGuards
            .EnsureAppointmentBelongsToMemberAsync(cmd.AppointmentId, cmd.MemberId, ct);

        if (guardRes.IsFailure)
            return Result<Unit>.FromError(guardRes);

        var (appointment, _) = guardRes.Value!;

        var customer = await _customerRepository
            .GetByIdAsync(appointment.CustomerId, ct);

        if (customer is null)
            throw new ScheduleException("Cliente não encontrado para o agendamento.");

        const decimal noShowPenalty = 30m;

        var changed = cmd.AttendanceStatus switch
        {
            AttendanceStatus.Attended =>
                appointment.MarkAttended(),

            AttendanceStatus.NoShow =>
                appointment.MarkNoShow(customer, noShowPenalty),

            _ => throw new ScheduleException("AttendanceStatus inválido para atualização.")
        };

        if (!changed)
            return Result<Unit>.NoContent();

        await _appointmentRepository.UpdateAsync(appointment, ct);
        await _customerRepository.UpdateAsync(customer, ct);

        return Result<Unit>.NoContent();
    }
}