using BaitaHora.Application.Features.Schedulings.Appointments.Cancel;
using BaitaHora.Application.Features.Schedulings.Appointments.Create;
using BaitaHora.Application.Features.Schedulings.Appointments.NoShow;
using BaitaHora.Application.Features.Schedulings.Appointments.Reschedule;
using BaitaHora.Contracts.DTOs.Schedulings;
using BaitaHora.Contracts.DTOs.Schedulings.Appointments;

namespace BaitaHora.Api.Mappers.Schedulings;

public static class AppointmentsApiMappers
{
    public static CreateAppointmentCommand ToCommand(this CreateAppointmentRequest r, Guid companyId)
        => new CreateAppointmentCommand
        {
            MemberId = r.MemberId,
            CustomerId = r.CustomerId,
            ServiceOfferingIds = r.ServiceOfferingIds ?? Array.Empty<Guid>(),
            StartsAtUtc = r.StartsAtUtc,
            DurationMinutes = r.DurationMinutes,
            CompanyId = companyId
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

    public static RescheduleAppointmentCommand ToCommand(
        this RescheduleAppointmentRequest r,
        Guid appointmentId,
        Guid companyId)
        => new RescheduleAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            NewStartsAtUtc = r.NewStartsAtUtc,
            NewDurationMinutes = r.NewDurationMinutes,
            CompanyId = companyId
        };

    public static CancelAppointmentCommand ToCommand(
        this CancelAppointmentRequest r,
        Guid appointmentId)
        => new CancelAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId
        };

    public static NoShowAppointmentCommand ToCommand(
        this NoShowAppointmentRequest r,
        Guid appointmentId,
        Guid companyId)
        => new NoShowAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            CompanyId = companyId
        };
}