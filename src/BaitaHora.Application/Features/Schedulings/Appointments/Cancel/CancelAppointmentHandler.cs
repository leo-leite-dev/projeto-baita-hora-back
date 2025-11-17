using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public interface ICancelAppointmentHandler
{
    Task<Result<Unit>> HandleAsync(
        CancelAppointmentCommand command,
        CancellationToken ct);
}