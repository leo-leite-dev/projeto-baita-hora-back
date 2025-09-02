namespace BaitaHora.Contracts.DTOs.Schedulings;

public sealed record CreateAppointmentRequest(
    Guid MemberId,
    Guid CustomerId,
    DateTime StartsAtUtc,
    int DurationMinutes
);