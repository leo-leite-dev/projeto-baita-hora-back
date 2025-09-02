namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed record CreateAppointmentResponse(
    Guid AppointmentId,
    Guid ScheduleId,
    Guid CompanyId,
    Guid MemberId,
    Guid CustomerId,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc
);