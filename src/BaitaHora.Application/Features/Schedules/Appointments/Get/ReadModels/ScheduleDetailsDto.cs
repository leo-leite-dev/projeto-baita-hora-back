using BaitaHora.Application.Features.Schedulings.Appointments.ReadModels;

namespace BaitaHora.Application.Features.Schedulings.Get.ReadModels;

public sealed record ScheduleDetailsDto
{
    public Guid ScheduleId { get; init; }
    public Guid MemberId { get; init; }
    public IEnumerable<AppointmentDto> Appointments { get; init; } = [];
}