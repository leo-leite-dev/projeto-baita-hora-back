namespace BaitaHora.Contracts.DTOS.Schedulings;

public sealed record GetAppointmentsResponse(
    Guid Id,
    string CustomerName,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string Status
);