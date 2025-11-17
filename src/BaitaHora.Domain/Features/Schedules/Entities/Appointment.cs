using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Customers;

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

    private Appointment() { }

    public static Appointment Create(Schedule schedule, Customer customer, IEnumerable<CompanyServiceOffering> serviceOfferings, DateTime startsAtUtc, TimeSpan duration)
    {
        if (schedule is null)
            throw new ScheduleException("Agenda inválida.");

        if (customer is null)
            throw new ScheduleException("Cliente inválido.");

        if (serviceOfferings is null)
            throw new ScheduleException("Serviços inválidos.");

        var services = serviceOfferings
            .Where(s => s is not null)
            .DistinctBy(s => s.Id)
            .ToArray();

        if (services.Length == 0)
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
            Status = AppointmentStatus.Pending
        };

        appointment._serviceOfferings.AddRange(services);

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

    public bool MarkCompleted()
    {
        if (Status == AppointmentStatus.Completed)
            return false;
        if (Status == AppointmentStatus.Cancelled)
            throw new ScheduleException("Não é possível concluir um agendamento cancelado.");

        Status = AppointmentStatus.Completed;
        Touch();
        return true;
    }

    public bool MarkNoShow()
    {
        if (Status == AppointmentStatus.NoShow)
            return false;

        Status = AppointmentStatus.NoShow;
        Touch();
        return true;
    }

    public bool Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            return false;
        if (Status == AppointmentStatus.Completed)
            throw new ScheduleException("Não é possível cancelar um agendamento concluído.");

        Status = AppointmentStatus.Cancelled;
        Touch();
        return true;
    }
}