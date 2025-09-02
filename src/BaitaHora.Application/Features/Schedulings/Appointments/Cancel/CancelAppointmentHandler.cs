using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public interface ICancelAppointmentHandler
{
    Task<Result<CancelAppointmentResponse>> HandleAsync(
        CancelAppointmentCommand command,
        CancellationToken ct);
}