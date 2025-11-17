using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Customers;

namespace BaitaHora.Domain.Features.Schedules.Entities;

public sealed class Schedule : EntityBase
{
    public Guid MemberId { get; private set; }

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Schedule() { }

    private Schedule(Guid memberId)
    {
        if (memberId == Guid.Empty)
            throw new ScheduleException("UserId inválido.");

        MemberId = memberId;
    }

    public static Schedule Create(Guid memberId) => new(memberId);

    public Appointment AddAppointment(
        Customer customer,
        CompanyServiceOffering serviceOffering,
        DateTime startsAtUtc,
        TimeSpan duration)
        => AddAppointment(customer, new[] { serviceOffering }, startsAtUtc, duration);


    public Appointment AddAppointment(
        Customer customer,
        IEnumerable<CompanyServiceOffering> serviceOfferings,
        DateTime startsAtUtc,
        TimeSpan duration)
    {
        if (customer is null)
            throw new ArgumentNullException(nameof(customer));

        if (serviceOfferings is null)
            throw new ScheduleException("Serviços inválidos.");

        if (duration <= TimeSpan.Zero)
            throw new ScheduleException("Duração deve ser maior que zero.");

        var appt = Appointment.Create(
            this,
            customer,
            serviceOfferings,
            startsAtUtc,
            duration);

        _appointments.Add(appt);
        Touch();
        return appt;
    }

    public bool RemoveAppointment(Appointment appointment)
    {
        if (appointment is null)
            throw new ArgumentNullException(nameof(appointment));

        var removed = _appointments.Remove(appointment);
        if (removed) Touch();

        return removed;
    }
}