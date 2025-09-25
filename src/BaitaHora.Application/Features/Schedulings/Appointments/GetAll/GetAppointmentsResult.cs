namespace BaitaHora.Application.Features.Schedulings.Appointments.GetAll;

public sealed record GetAppointmentsResult(
    Guid Id,
    string CustomerName,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string Status
);