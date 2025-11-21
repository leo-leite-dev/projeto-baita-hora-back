using BaitaHora.Application.Features.Schedules.Appointments.Reschedule;
using BaitaHora.Application.Features.Schedules.Appointments.UpdateStatus;
using BaitaHora.Application.Features.Schedules.Appointments.Cancel;
using BaitaHora.Application.Features.Schedules.Appointments.Create;
using BaitaHora.Contracts.DTOs.Schedules.Appointments;
using DomainAttendanceStatus = BaitaHora.Domain.Features.Schedules.Enums.AttendanceStatus;

namespace BaitaHora.Api.Mappers.Schedules.Appointments;

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
        Guid appointmentId,
        Guid companyId)
        => new CancelAppointmentCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            CompanyId = companyId
        };

    public static UpdateAttendanceStatusCommand ToCommand(
        this UpdateAttendanceStatusRequest r,
        Guid appointmentId,
        Guid companyId)
        => new UpdateAttendanceStatusCommand
        {
            MemberId = r.MemberId,
            AppointmentId = appointmentId,
            AttendanceStatus = (DomainAttendanceStatus)r.AttendanceStatus,
            CompanyId = companyId
        };
}