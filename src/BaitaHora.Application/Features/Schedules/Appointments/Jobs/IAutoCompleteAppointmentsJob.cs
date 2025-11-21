namespace BaitaHora.Application.Features.Schedules.Appointments.Jobs;

public interface IAutoCompleteAppointmentsJob
{
    Task RunAsync(CancellationToken ct = default);
}