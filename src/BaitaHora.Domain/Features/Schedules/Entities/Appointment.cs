using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Domain.Features.Schedules.Enums;

namespace BaitaHora.Domain.Features.Schedules.Entities;

public sealed class Appointment : EntityBase
{
    public Guid ScheduleId { get; private set; }
    public Guid CustomerId { get; private set; }

    private readonly List<CompanyServiceOffering> _serviceOfferings = new();
    public IReadOnlyCollection<CompanyServiceOffering> ServiceOfferings => _serviceOfferings.AsReadOnly();

    public DateTime StartsAtUtc { get; private set; }
    public TimeSpan Duration { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;
    public AttendanceStatus AttendanceStatus { get; private set; } = AttendanceStatus.Unknown;

    private Appointment() { }

    public static Appointment Create(
        Schedule schedule,
        Customer customer,
        IEnumerable<CompanyServiceOffering> serviceOfferings,
        DateTime startsAtUtc,
        TimeSpan duration)
    {
        if (schedule is null)
            throw new ScheduleException("Agenda inválida.");

        if (customer is null)
            throw new ScheduleException("Cliente inválido.");

        if (serviceOfferings is null)
            throw new ScheduleException("Serviços inválidos.");

        if (!serviceOfferings.Any())
            throw new ScheduleException("Agendamento deve ter ao menos um serviço.");

        if (startsAtUtc == default)
            throw new ScheduleException("Data/hora de início inválida.");

        if (duration <= TimeSpan.Zero)
            throw new ScheduleException("Duração deve ser maior que zero.");

        var appointment = new Appointment
        {
            ScheduleId = schedule.Id,
            CustomerId = customer.Id,
            StartsAtUtc = startsAtUtc,
            Duration = duration,
            Status = AppointmentStatus.Pending,
            AttendanceStatus = AttendanceStatus.Unknown
        };

        appointment.AddServiceOfferings(serviceOfferings);

        return appointment;
    }

    public DateTime EndsAtUtc => StartsAtUtc.Add(Duration);

    internal void AddServiceOffering(CompanyServiceOffering service)
    {
        if (service is null)
            throw new ScheduleException("Serviço inválido.");

        if (_serviceOfferings.Any(s => s.Id == service.Id))
            return;

        _serviceOfferings.Add(service);
        Touch();
    }

    internal void AddServiceOfferings(IEnumerable<CompanyServiceOffering>? services)
    {
        if (services is null)
            return;

        foreach (var s in services)
            AddServiceOffering(s);
    }

    public bool Reschedule(DateTime newStart, TimeSpan newDuration)
    {
        if (AttendanceStatus == AttendanceStatus.NoShow)
            throw new ScheduleException("Agendamentos marcados como falta (no-show) não podem ser remarcados.");

        if (Status != AppointmentStatus.Pending)
            throw new ScheduleException("Só é possível reagendar agendamentos pendentes.");

        if (newStart == default)
            throw new ScheduleException("Nova data inválida.");

        if (newDuration <= TimeSpan.Zero)
            throw new ScheduleException("Duração inválida.");

        if (newStart == StartsAtUtc && newDuration == Duration)
            return false;

        StartsAtUtc = newStart;
        Duration = newDuration;
        Touch();
        return true;
    }

    public bool Finish()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new ScheduleException("Não é possível finalizar um agendamento cancelado.");

        if (Status == AppointmentStatus.Finished)
            return false;

        Status = AppointmentStatus.Finished;
        Touch();
        return true;
    }

    public bool MarkAttended()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new ScheduleException("Não é possível registrar presença em um agendamento cancelado.");

        if (AttendanceStatus == AttendanceStatus.Attended)
            return false;

        if (Status == AppointmentStatus.Pending)
            Status = AppointmentStatus.Finished;

        AttendanceStatus = AttendanceStatus.Attended;
        Touch();
        return true;
    }

    public bool MarkNoShow()
    {
        if (Status == AppointmentStatus.Cancelled)
            throw new ScheduleException("Não é possível marcar falta em um agendamento cancelado.");

        if (AttendanceStatus == AttendanceStatus.NoShow)
            return false;

        if (Status == AppointmentStatus.Pending)
            Status = AppointmentStatus.Finished;

        AttendanceStatus = AttendanceStatus.NoShow;
        Touch();
        return true;
    }

    public bool Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            return false;

        if (Status == AppointmentStatus.Finished)
            throw new ScheduleException("Não é possível cancelar um agendamento finalizado.");

        Status = AppointmentStatus.Cancelled;
        AttendanceStatus = AttendanceStatus.Unknown;
        Touch();
        return true;
    }
}