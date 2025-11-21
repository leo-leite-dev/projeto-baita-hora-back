namespace BaitaHora.Application.Features.Schedules.Appointments.Reschedule;

public sealed record RescheduleAppointmentResponse(
    Guid AppointmentId,
    Guid ScheduleId,
    Guid CompanyId,
    Guid MemberId,
    Guid CustomerId,
    DateTime PreviousStartsAtUtc,
    DateTime PreviousEndsAtUtc,
    DateTime NewStartsAtUtc,
    DateTime NewEndsAtUtc
);
