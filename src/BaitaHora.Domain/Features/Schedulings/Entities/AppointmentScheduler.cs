using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Domain.Entities.Scheduling;

public class Appointment : Entity
{
    public Guid ScheduleId { get; private set; }
    public Schedule Schedule { get; private set; } = null!;

    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }

    // public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;
    // public AppointmentCreatedBy CreatedBy { get; private set; } = AppointmentCreatedBy.Staff;

    public Guid CustomerId { get; private set; }

    public string CustomerDisplayName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;

    public Guid? ServiceId { get; private set; }

    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }

    private Appointment() { }

    internal static Appointment CreateForCustomer(Guid scheduleId, DateTime startsAtUtc, TimeSpan duration,
     Customer? customer, Guid? serviceId, string? notes)
    {
        if (scheduleId == Guid.Empty)
            throw new SchedulingException("Agenda inválida.");

        if (customer is null)
            throw new SchedulingException("Cliente obrigatório.");

        if (duration <= TimeSpan.Zero)
            throw new SchedulingException("Duração inválida.");

        var display = customer.CustomerName.Value;
        var phone = customer.CustomerPhone.Value;

        if (string.IsNullOrWhiteSpace(display) || string.IsNullOrWhiteSpace(phone))
            throw new SchedulingException("Dados do cliente incompletos.");

        return new Appointment
        {
            ScheduleId = scheduleId,
            StartsAtUtc = startsAtUtc,
            EndsAtUtc = startsAtUtc + duration,
            CustomerId = customer.Id,
            CustomerDisplayName = display,
            CustomerPhone = phone,
            ServiceId = serviceId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
    
    internal bool Reschedule(DateTime newStartUtc, TimeSpan newDuration)
    {
        if (newDuration <= TimeSpan.Zero) throw new SchedulingException("Duração inválida.");
        var newEnd = newStartUtc + newDuration;

        if (newStartUtc == StartsAtUtc && newEnd == EndsAtUtc) return false;

        StartsAtUtc = newStartUtc;
        EndsAtUtc = newEnd;
        Touch();
        return true;
    }

    public bool UpdateNotes(string? notes)
    {
        var normalized = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (string.Equals(Notes, normalized, StringComparison.Ordinal))
            return false;
        Notes = normalized;

        return true;
    }

    public bool Cancel(string reason)
    {
        var normalized = (reason ?? "").Trim();

        if (normalized.Length == 0)
            throw new SchedulingException("Motivo obrigatório.");

        if (string.Equals(CancellationReason, normalized, StringComparison.Ordinal))
            return false;
        CancellationReason = normalized;

        return true;
    }
}