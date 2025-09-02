namespace BaitaHora.Contracts.DTOs.Schedulings;

public sealed record CancelAppointmentRequest(
    Guid MemberId
);