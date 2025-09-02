namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public sealed record CancelAppointmentResponse(
    Guid AppointmentId,
    string Status
);