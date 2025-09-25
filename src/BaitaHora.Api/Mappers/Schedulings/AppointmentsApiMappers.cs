using BaitaHora.Application.Features.Schedulings.Appointments.Cancel;
using BaitaHora.Application.Features.Schedulings.Appointments.Complete;
using BaitaHora.Application.Features.Schedulings.Appointments.Create;
using BaitaHora.Application.Features.Schedulings.Appointments.GetAll;
using BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;
using BaitaHora.Contracts.DTOs.Schedulings;
using BaitaHora.Contracts.DTOS.Schedulings;

namespace BaitaHora.Api.Mappers.Schedulings;

public static class AppointmentsApiMappers
{
    public static CreateAppointmentCommand ToCommand(this CreateAppointmentRequest r)
        => new CreateAppointmentCommand
        {
            MemberId = r.MemberId,
            CustomerId = r.CustomerId,
            StartsAtUtc = r.StartsAtUtc,
            DurationMinutes = r.DurationMinutes
        };

    public static RescheduleAppointmentCommand ToCommand(
        this RescheduleAppointmentRequest r,
        Guid appointmentId)
        => new RescheduleAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            NewStartsAtUtc = r.NewStartsAtUtc,
            NewDurationMinutes = r.NewDurationMinutes
        };

    public static CompleteAppointmentCommand ToCommand(
        this CompleteAppointmentRequest r,
        Guid appointmentId)
        => new CompleteAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId
        };

    public static CancelAppointmentCommand ToCommand(
        this CancelAppointmentRequest r,
        Guid appointmentId)
        => new CancelAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId
        };

    public static GetAppointmentsResponse ToResponse(this GetAppointmentsResult r)
        => new GetAppointmentsResponse(
            r.Id,
            r.CustomerName,
            r.StartsAtUtc,
            r.EndsAtUtc,
            r.Status.ToString()
        );

    public static IReadOnlyList<GetAppointmentsResponse> ToResponse(this IEnumerable<GetAppointmentsResult> items)
        => items.Select(ToResponse).ToList();
}