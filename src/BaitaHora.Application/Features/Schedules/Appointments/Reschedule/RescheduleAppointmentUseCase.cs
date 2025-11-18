using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;

public sealed class RescheduleAppointmentUseCase
{
    private readonly IScheduleGuards _scheduleGuards;
    private readonly IAppointmentRepository _appointmentRepository;

    public RescheduleAppointmentUseCase(
        IScheduleGuards scheduleGuards,
        IAppointmentRepository appointmentRepository)
    {
        _scheduleGuards = scheduleGuards;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<RescheduleAppointmentResponse>> HandleAsync(
        RescheduleAppointmentCommand cmd, CancellationToken ct)
    {
        var guardRes = await _scheduleGuards
            .EnsureAppointmentBelongsToMemberAsync(cmd.AppointmentId, cmd.MemberId, ct);

        if (guardRes.IsFailure)
            return Result<RescheduleAppointmentResponse>.FromError(guardRes);

        var (appt, schedule) = guardRes.Value!;

        if (cmd.NewDurationMinutes <= 0)
            return Result<RescheduleAppointmentResponse>.BadRequest("Duração deve ser maior que zero.");

        var previousStart = appt.StartsAtUtc;
        var previousEnd = appt.EndsAtUtc;

        var newDuration = TimeSpan.FromMinutes(cmd.NewDurationMinutes);

        bool changed;
        try
        {
            changed = appt.Reschedule(cmd.NewStartsAtUtc, newDuration);
        }
        catch (ScheduleException ex)
        {
            return Result<RescheduleAppointmentResponse>.BadRequest(ex.Message);
        }

        if (!changed)
            return Result<RescheduleAppointmentResponse>.NoContent();

        await _appointmentRepository.UpdateAsync(appt, ct);

        var response = new RescheduleAppointmentResponse(
            AppointmentId: appt.Id,
            ScheduleId: schedule.Id,
            CompanyId: cmd.CompanyId,
            MemberId: cmd.MemberId,
            CustomerId: appt.CustomerId,
            PreviousStartsAtUtc: previousStart,
            PreviousEndsAtUtc: previousEnd,
            NewStartsAtUtc: appt.StartsAtUtc,
            NewEndsAtUtc: appt.EndsAtUtc
        );

        return Result<RescheduleAppointmentResponse>.Ok(response);
    }
}