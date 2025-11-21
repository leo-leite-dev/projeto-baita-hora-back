using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;

namespace BaitaHora.Application.Features.Schedules.Get.ReadModels;

public record ScheduleDetailsDto(
    Guid ScheduleId,
    Guid MemberId,
    IEnumerable<AppointmentDto> Appointments
);

public sealed record ScheduleByMemberDetailsDto(
    Guid ScheduleId,
    Guid MemberId,
    IEnumerable<AppointmentDto> Appointments,
    string MemberName,
    string PositionName
) : ScheduleDetailsDto(ScheduleId, MemberId, Appointments);
