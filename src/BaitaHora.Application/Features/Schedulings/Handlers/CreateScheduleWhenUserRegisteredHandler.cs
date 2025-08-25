using BaitaHora.Application.Common.Events;
using BaitaHora.Application.Common.Time;
using BaitaHora.Application.Features.Schedules;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Users.Events;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Handlers;

public sealed class CreateScheduleWhenUserRegisteredHandler
    : INotificationHandler<DomainEventNotification<UserRegisteredDomainEvent>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IClock _clock;

    public CreateScheduleWhenUserRegisteredHandler(
        IScheduleRepository scheduleRepository,
        IClock clock)
    {
        _scheduleRepository = scheduleRepository;
        _clock = clock;
    }

    public async Task Handle(DomainEventNotification<UserRegisteredDomainEvent> notification, CancellationToken ct)
    {
        var @event = notification.DomainEvent;

        if (await _scheduleRepository.ExistsForUserAsync(@event.UserId, ct))
            return;

        var schedule = Schedule.Create(@event.UserId, null, _clock.UtcNow);
        await _scheduleRepository.AddAsync(schedule, ct);
    }
}