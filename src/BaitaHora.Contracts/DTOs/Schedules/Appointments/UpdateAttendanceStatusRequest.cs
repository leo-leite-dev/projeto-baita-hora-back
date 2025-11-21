using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Schedules.Appointments;

public sealed record UpdateAttendanceStatusRequest(
    Guid MemberId,
    Guid AppointmentId,
    AttendanceStatus AttendanceStatus
);
