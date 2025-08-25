using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Customers;

namespace BaitaHora.Domain.Features.Schedules.Entities;

public sealed class Schedule : Entity
{
    private Schedule() { }

    public Guid UserId { get; private set; }
    public string TimeZone { get; private set; } = "America/Sao_Paulo";

    public TimeSpan SlotDuration { get; private set; } = TimeSpan.FromMinutes(30);
    public TimeSpan WindowStartLocal { get; private set; } = new(0, 0, 0);
    public TimeSpan WindowEndLocal { get; private set; } = new(23, 59, 59);
    public TimeSpan SlotAnchorLocal { get; private set; } = new(0, 0, 0);

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static Schedule Create(Guid userId, string? timeZone, DateTime _nowUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("Invalid UserId.", nameof(userId));

        return new Schedule
        {
            UserId = userId,
            TimeZone = string.IsNullOrWhiteSpace(timeZone) ? "America/Sao_Paulo" : timeZone!
        };
    }

    public bool SetTimeZone(string tz, DateTime _nowUtc)
    {
        var normalized = tz?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) || string.Equals(TimeZone, normalized, StringComparison.Ordinal))
            return false;

        TimeZone = normalized!;
        Touch();
        return true;
    }

    public bool ConfigureWorkingWindow(TimeSpan startLocal, TimeSpan endLocal, TimeSpan? anchorLocal = null)
    {
        if (startLocal >= endLocal)
            throw new SchedulingException("Janela de atendimento inválida.");

        var changed = false;

        if (WindowStartLocal != startLocal)
        { WindowStartLocal = startLocal; changed = true; }

        if (WindowEndLocal != endLocal)
        { WindowEndLocal = endLocal; changed = true; }

        if (anchorLocal.HasValue && SlotAnchorLocal != anchorLocal.Value)
        {
            SlotAnchorLocal = anchorLocal.Value;
            changed = true;
        }

        if (changed) Touch();
        return changed;
    }

    public bool SetSlotDuration(TimeSpan slot)
    {
        if (slot <= TimeSpan.Zero)
            throw new SchedulingException("Slot inválido.");

        if (SlotDuration == slot)
            return false;
        SlotDuration = slot;
        Touch();
        return true;
    }
    public Appointment AddAppointment(Customer customer, DateTime startsAtUtc, CompanyService service, Guid professionalPositionId, string? notes = null)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));

        if (service is null || !service.IsActive)
            throw new SchedulingException("Serviço inválido ou inativo.");

        var isLinked = service.PositionLinks.Any(l => l.PositionId == professionalPositionId);
        if (!isLinked)
            throw new SchedulingException("O profissional não executa este serviço.");

        var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        var startsLocal = TimeZoneInfo.ConvertTimeFromUtc(startsAtUtc, tz);
        var tod = startsLocal.TimeOfDay;

        if (tod < WindowStartLocal || tod >= WindowEndLocal)
            throw new SchedulingException("Fora da janela de atendimento.");

        if (!IsAlignedToSlot(startsLocal, SlotAnchorLocal, SlotDuration))
            throw new SchedulingException($"Horário deve respeitar múltiplos de {SlotDuration.TotalMinutes} minutos a partir de {SlotAnchorLocal}.");

        var endsAtUtc = startsAtUtc + SlotDuration;

        var hasOverlap = _appointments.Any(a => startsAtUtc < a.EndsAtUtc && endsAtUtc > a.StartsAtUtc);
        if (hasOverlap)
            throw new SchedulingException("Há um compromisso no período selecionado.");

        var appt = Appointment.CreateForCustomer(
            Id, startsAtUtc, SlotDuration, customer, service.Id, service.ServiceName, notes);

        _appointments.Add(appt);
        Touch();
        return appt;
    }

    private static bool IsAlignedToSlot(DateTime localStart, TimeSpan anchor, TimeSpan slot)
    {
        var minutes = localStart.Hour * 60 + localStart.Minute;
        var anchorMin = (int)anchor.TotalMinutes;
        var delta = minutes - anchorMin;
        return delta >= 0
            && delta % (int)slot.TotalMinutes == 0
            && localStart.Second == 0
            && localStart.Millisecond == 0;
    }

    public IEnumerable<DateTime> GetAvailableSlotsUtc(DateOnly day)
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        var date = day.ToDateTime(TimeOnly.MinValue);
        var startLocal = date + WindowStartLocal;
        var endLocal = date + WindowEndLocal;

        for (var current = startLocal; current + SlotDuration <= endLocal; current = current.Add(SlotDuration))
        {
            if (!IsAlignedToSlot(current, SlotAnchorLocal, SlotDuration)) continue;

            var currentUtc = TimeZoneInfo.ConvertTimeToUtc(current, tz);
            var candidateEndUtc = currentUtc + SlotDuration;

            var hasOverlap = _appointments.Any(a => currentUtc < a.EndsAtUtc && candidateEndUtc > a.StartsAtUtc);
            if (!hasOverlap) yield return currentUtc;
        }
    }
}