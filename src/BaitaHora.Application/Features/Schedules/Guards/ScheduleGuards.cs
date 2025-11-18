using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.Features.Schedules.Guards;

public sealed class ScheduleGuards : IScheduleGuards
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleGuards(
        IAppointmentRepository appointmentRepository,
        IScheduleRepository scheduleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<(Appointment Appointment, Schedule Schedule)>>
        EnsureAppointmentBelongsToMemberAsync(Guid appointmentId, Guid memberId, CancellationToken ct)
    {
        if (appointmentId == Guid.Empty)
            return Result<(Appointment, Schedule)>.BadRequest("AppointmentId inválido.");

        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct);
        if (appt is null)
            return Result<(Appointment, Schedule)>.NotFound("Agendamento não encontrado.");

        var schedule = await _scheduleRepository.GetByIdAsync(appt.ScheduleId, ct);
        if (schedule is null || schedule.MemberId != memberId)
            return Result<(Appointment, Schedule)>.Forbidden("Agendamento não pertence a este membro.");

        return Result<(Appointment, Schedule)>.Ok((appt, schedule));
    }
}