using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Schedulings;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public sealed class CancelAppointmentUseCase : ICancelAppointmentHandler
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public CancelAppointmentUseCase(
        IAppointmentRepository appointmentRepository,
        IScheduleRepository scheduleRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<CancelAppointmentResponse>> HandleAsync(
        CancelAppointmentCommand cmd,
        CancellationToken ct)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(cmd.AppointmentId, ct);
        if (appointment is null)
            return Result<CancelAppointmentResponse>.NotFound("Agendamento não encontrado.");

        var schedule = await _scheduleRepository.GetByIdAsync(appointment.ScheduleId, ct);
        if (schedule is null || schedule.MemberId != cmd.MemberId)
            return Result<CancelAppointmentResponse>.Forbidden("Agendamento não pertence a este membro.");


        var changed = appointment.Cancel();
        if (!changed)
            return Result<CancelAppointmentResponse>.NoContent();

        await _appointmentRepository.UpdateAsync(appointment, ct);

        return Result<CancelAppointmentResponse>.Ok(new CancelAppointmentResponse(appointment.Id, appointment.Status.ToString()
        ));
    }
}