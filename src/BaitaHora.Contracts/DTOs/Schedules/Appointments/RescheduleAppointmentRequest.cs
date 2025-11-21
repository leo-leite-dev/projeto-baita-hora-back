namespace BaitaHora.Contracts.DTOs.Schedules.Appointments;

public sealed record RescheduleAppointmentRequest(
    Guid MemberId,
    DateTime NewStartsAtUtc,
    int NewDurationMinutes
);