using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Schedulings;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public sealed class CancelAppointmentUseCase
{
    private readonly IScheduleGuards _scheduleGuards;
    private readonly IAppointmentRepository _appointmentRepository;

    public CancelAppointmentUseCase(
        IScheduleGuards scheduleGuards,
        IAppointmentRepository appointmentRepository)
    {
        _scheduleGuards = scheduleGuards;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<Unit>> HandleAsync(
        CancelAppointmentCommand cmd,
        CancellationToken ct)
    {
        var guardRes = await _scheduleGuards
            .EnsureAppointmentBelongsToMemberAsync(cmd.AppointmentId, cmd.MemberId, ct);

        if (guardRes.IsFailure)
            return Result<Unit>.FromError(guardRes);

        var (appointment, _) = guardRes.Value!;

        var changed = appointment.Cancel();
        if (!changed)
            return Result<Unit>.NoContent();

        await _appointmentRepository.UpdateAsync(appointment, ct);

        return Result<Unit>.NoContent();
    }
}