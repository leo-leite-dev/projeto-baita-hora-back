namespace BaitaHora.Application.Features.Schedulings.Appointments.Jobs;

public interface IAutoCompleteAppointmentsJob
{
    Task RunAsync(CancellationToken ct = default);
}