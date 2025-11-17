using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Schedulings;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.NoShow;

public sealed class NoShowAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public NoShowAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IScheduleRepository scheduleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<Unit>> HandleAsync(
        NoShowAppointmentCommand cmd,
        CancellationToken ct)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(cmd.AppointmentId, ct);
        if (appointment is null)
            return Result<Unit>.NotFound("Agendamento não encontrado.");

        var schedule = await _scheduleRepository.GetByIdAsync(appointment.ScheduleId, ct);
        if (schedule is null || schedule.MemberId != cmd.MemberId)
            return Result<Unit>.Forbidden("Agendamento não pertence ao membro informado.");

        var changed = appointment.MarkNoShow();
        if (!changed)
            return Result<Unit>.NoContent();

        await _appointmentRepository.UpdateAsync(appointment, ct);

        return Result<Unit>.NoContent();
    }
}