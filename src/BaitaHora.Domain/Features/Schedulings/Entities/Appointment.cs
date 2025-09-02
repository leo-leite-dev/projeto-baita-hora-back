using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Customers;

namespace BaitaHora.Domain.Features.Schedules.Entities;

public sealed class Appointment : EntityBase
{
    public Guid ScheduleId { get; private set; }
    public Guid CustomerId { get; private set; }

    public DateTime StartsAtUtc { get; private set; }
    public TimeSpan Duration { get; private set; }

    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

    private Appointment() { }

    private Appointment(Schedule schedule, Customer customer, DateTime startsAtUtc, TimeSpan duration)
    {
        if (schedule is null)
            throw new ScheduleException("Agenda inválida.");

        if (customer is null)
            throw new ScheduleException("Cliente inválido.");

        if (startsAtUtc == default)
            throw new ScheduleException("Data/hora de início inválida.");

        if (duration <= TimeSpan.Zero)
            throw new ScheduleException("Duração deve ser maior que zero.");

        ScheduleId = schedule.Id;
        CustomerId = customer.Id;
        StartsAtUtc = startsAtUtc;
        Duration = duration;
        Status = AppointmentStatus.Pending;
    }

    public static Appointment Create(Schedule schedule, Customer customer, DateTime startsAtUtc, TimeSpan duration)
        => new(schedule, customer, startsAtUtc, duration);

    public DateTime EndsAtUtc => StartsAtUtc.Add(Duration);

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