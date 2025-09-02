using BaitaHora.Application.Common.Events;
using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Users.Events;
using Microsoft.Extensions.Logging;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Handlers;

public sealed class CreateScheduleWhenUserRegisteredHandler
    : INotificationHandler<DomainEventNotification<UserRegisteredDomainEvent>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public CreateScheduleWhenUserRegisteredHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task Handle(
        DomainEventNotification<UserRegisteredDomainEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        if (await _scheduleRepository.ExistsForUserAsync(e.UserId, ct))
            return;


        var schedule = Schedule.Create(e.UserId);
        await _scheduleRepository.AddAsync(schedule, ct);
    }
}