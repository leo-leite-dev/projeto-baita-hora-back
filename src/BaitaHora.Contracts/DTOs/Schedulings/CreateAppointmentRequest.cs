namespace BaitaHora.Contracts.DTOs.Schedulings.Appointments;

public sealed record CreateAppointmentRequest(
    Guid MemberId,
    Guid CustomerId,
    IReadOnlyCollection<Guid> ServiceOfferingIds,
    DateTime StartsAtUtc,
    int DurationMinutes
);