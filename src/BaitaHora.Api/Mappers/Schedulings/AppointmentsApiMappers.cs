using BaitaHora.Application.Features.Schedulings.Appointments.Cancel;
using BaitaHora.Application.Features.Schedulings.Appointments.Complete;
using BaitaHora.Application.Features.Schedulings.Appointments.Create;
using BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;
using BaitaHora.Contracts.DTOs.Schedulings;

namespace BaitaHora.Api.Mappers.Schedulings;

public static class AppointmentsApiMappers
{
    public static CreateAppointmentCommand ToCommand(
        this CreateAppointmentRequest r, Guid companyId)
        => new CreateAppointmentCommand
        {
            CompanyId = companyId,
            MemberId = r.MemberId,
            CustomerId = r.CustomerId,
            StartsAtUtc = r.StartsAtUtc,
            DurationMinutes = r.DurationMinutes
        };

    public static RescheduleAppointmentCommand ToCommand(
        this RescheduleAppointmentRequest r,
        Guid companyId,
        Guid appointmentId)
        => new RescheduleAppointmentCommand
        {
            CompanyId = companyId,
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            NewStartsAtUtc = r.NewStartsAtUtc,
            NewDurationMinutes = r.NewDurationMinutes
        };

    public static CompleteAppointmentCommand ToCommand(
        this CompleteAppointmentRequest r, Guid companyId, Guid appointmentId)
        => new CompleteAppointmentCommand
        {
            CompanyId = companyId,
            MemberId = r.MemberId,
            AppointmentId = appointmentId
        };

    public static CancelAppointmentCommand ToCommand(
        this CancelAppointmentRequest r, Guid companyId, Guid appointmentId)
        => new CancelAppointmentCommand
        {
            CompanyId = companyId,
            MemberId = r.MemberId,
            AppointmentId = appointmentId
        };
}