namespace BaitaHora.Application.Features.Schedulings.Appointments.ReadModels;

public record AppointmentDtoBase
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;

    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;

    public IReadOnlyList<string> ServiceOfferingNames { get; init; } = Array.Empty<string>();
}

public sealed record AppointmentDto : AppointmentDtoBase
{
    public Guid CustomerId { get; init; }
    public IReadOnlyList<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public DateTime StartsAtUtc { get; init; }
    public DateTime EndsAtUtc { get; init; }
    public int DurationMinutes { get; init; }
}