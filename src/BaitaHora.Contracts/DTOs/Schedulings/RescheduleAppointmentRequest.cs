namespace BaitaHora.Contracts.DTOs.Schedulings;

public sealed record RescheduleAppointmentRequest(
    Guid MemberId,
    DateTime NewStartsAtUtc,
    int NewDurationMinutes
);