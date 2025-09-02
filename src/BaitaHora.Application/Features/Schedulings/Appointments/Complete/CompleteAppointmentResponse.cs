namespace BaitaHora.Application.Features.Schedules.CompleteAppointment;

public sealed record CompleteAppointmentResponse(
    Guid AppointmentId,
    string Status
);