using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Appointments.Complete;
using BaitaHora.Application.IRepositories.Schedulings;

namespace BaitaHora.Application.Features.Schedules.CompleteAppointment;

public sealed class CompleteAppointmentUseCase : ICompleteAppointmentHandler
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public CompleteAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IScheduleRepository scheduleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<CompleteAppointmentResponse>> HandleAsync(CompleteAppointmentCommand cmd, CancellationToken ct)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(cmd.AppointmentId, ct);
        if (appointment is null)
            return Result<CompleteAppointmentResponse>.NotFound("Agendamento não encontrado.");

        var schedule = await _scheduleRepository.GetByIdAsync(appointment.ScheduleId, ct);
        if (schedule is null || schedule.MemberId != cmd.MemberId)
            return Result<CompleteAppointmentResponse>.Forbidden("Agendamento não pertence a este membro.");

        var changed = appointment.MarkCompleted();
        if (!changed)
            return Result<CompleteAppointmentResponse>.NoContent();

        await _appointmentRepository.UpdateAsync(appointment, ct);

        return Result<CompleteAppointmentResponse>.Ok(new CompleteAppointmentResponse(appointment.Id, appointment.Status.ToString()
        ));
    }
}