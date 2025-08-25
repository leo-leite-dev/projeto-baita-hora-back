using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Customers;

namespace BaitaHora.Domain.Features.Schedules.Entities;

public sealed class Schedule : Entity
{
    private Schedule() { }

    public Guid UserId { get; private set; }
    public string TimeZone { get; private set; } = "America/Sao_Paulo";

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static Schedule Create(Guid userId, string? timeZone, DateTime nowUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("Invalid UserId.", nameof(userId));

        return new Schedule
        {
            UserId = userId,
            TimeZone = string.IsNullOrWhiteSpace(timeZone) ? "America/Sao_Paulo" : timeZone!,
        };
    }

    public bool SetTimeZone(string tz, DateTime nowUtc)
    {
        var normalized = tz?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || string.Equals(TimeZone, normalized, StringComparison.Ordinal))
            return false;

        TimeZone = normalized!;
        Touch();
        return true;
    }

    public Appointment AddAppointment(Customer customer, DateTime startsAtUtc, TimeSpan duration, Guid? serviceId = null, string? notes = null)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));

        if (duration <= TimeSpan.Zero)
            throw new SchedulingException("Duração inválida.");

        var newEnd = startsAtUtc + duration;

        var hasOverlap = _appointments.Any(a =>
            startsAtUtc < a.EndsAtUtc && newEnd > a.StartsAtUtc
        );
        
        if (hasOverlap)
            throw new SchedulingException("Há um compromisso no período selecionado.");

        var appointment = Appointment.CreateForCustomer(Id, startsAtUtc, duration, customer, serviceId, notes);

        _appointments.Add(appointment);

        return appointment;
    }
}