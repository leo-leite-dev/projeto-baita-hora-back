using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;

namespace BaitaHora.Application.Features.Companies.Stats.ReadModels;

public sealed class MemberAppointmentsDto
{
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = string.Empty;
    public string PositionName { get; init; } = string.Empty;

    public IReadOnlyList<AppointmentDto> Appointments { get; init; }
        = Array.Empty<AppointmentDto>();
}