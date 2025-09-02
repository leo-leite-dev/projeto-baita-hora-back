using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Appointments.Complete;

namespace BaitaHora.Application.Features.Schedules.CompleteAppointment;

public interface ICompleteAppointmentHandler
{
    Task<Result<CompleteAppointmentResponse>> HandleAsync(
        CompleteAppointmentCommand command, 
        CancellationToken ct);
}