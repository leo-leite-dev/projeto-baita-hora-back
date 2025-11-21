namespace BaitaHora.Contracts.DTOs.Schedules.Appointments;

public sealed record CancelAppointmentRequest(
    Guid MemberId
);