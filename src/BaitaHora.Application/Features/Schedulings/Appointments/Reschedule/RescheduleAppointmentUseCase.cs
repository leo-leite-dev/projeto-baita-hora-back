using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Schedulings;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;

public sealed class RescheduleAppointmentUseCase
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public RescheduleAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IScheduleRepository scheduleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<RescheduleAppointmentResponse>> HandleAsync(
        RescheduleAppointmentCommand cmd, CancellationToken ct)
    {
        var appt = await _appointmentRepository.GetByIdAsync(cmd.AppointmentId, ct);
        if (appt is null)
            return Result<RescheduleAppointmentResponse>.NotFound("Agendamento não encontrado.");

        if (appt.Status == AppointmentStatus.NoShow)
            return Result<RescheduleAppointmentResponse>.BadRequest(
                "Agendamentos marcados como no-show não podem ser remarcados.");
                
        var schedule = await _scheduleRepository.GetByIdAsync(appt.ScheduleId, ct);
        if (schedule is null)
            return Result<RescheduleAppointmentResponse>.NotFound("Agenda não encontrada.");

        if (schedule.MemberId != cmd.MemberId)
            return Result<RescheduleAppointmentResponse>.Forbidden("Agendamento não pertence a este profissional.");

        if (cmd.NewDurationMinutes <= 0)
            return Result<RescheduleAppointmentResponse>.BadRequest("Duração deve ser maior que zero.");

        var previousStart = appt.StartsAtUtc;
        var previousEnd = appt.EndsAtUtc;

        var newDuration = TimeSpan.FromMinutes(cmd.NewDurationMinutes);

        var changed = appt.Reschedule(cmd.NewStartsAtUtc, newDuration);

        if (!changed)
            return Result<RescheduleAppointmentResponse>.NoContent();

        await _appointmentRepository.UpdateAsync(appt, ct);

        var response = new RescheduleAppointmentResponse(
            AppointmentId: appt.Id,
            ScheduleId: appt.ScheduleId,
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