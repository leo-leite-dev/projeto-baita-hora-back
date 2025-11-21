using BaitaHora.Domain.Features.Schedules.Enums;

namespace BaitaHora.Application.Features.Schedules.Appointments.ReadModels;

public sealed record AppointmentDto
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerPhone { get; init; } = string.Empty;

    public IReadOnlyList<string> ServiceOfferingNames { get; init; } = Array.Empty<string>();
    public AttendanceStatus AttendanceStatus { get; init; } = AttendanceStatus.Unknown;
    public IReadOnlyList<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public DateTime StartsAtUtc { get; init; }
    public DateTime EndsAtUtc { get; init; }
    public int DurationMinutes { get; init; }
}