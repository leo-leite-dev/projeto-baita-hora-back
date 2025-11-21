namespace BaitaHora.Contracts.DTOs.Schedules.Appointments;

public sealed record NoShowAppointmentRequest(
    Guid MemberId
);