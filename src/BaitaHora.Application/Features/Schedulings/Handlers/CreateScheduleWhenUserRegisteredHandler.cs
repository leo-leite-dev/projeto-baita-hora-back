using BaitaHora.Application.Common.Events;
using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Companies.Events;
using BaitaHora.Domain.Features.Schedules.Entities;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Handlers;

public sealed class CreateScheduleWhenMemberCreatedHandler
    : INotificationHandler<DomainEventNotification<CompanyMemberCreatedDomainEvent>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public CreateScheduleWhenMemberCreatedHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task Handle(
        DomainEventNotification<CompanyMemberCreatedDomainEvent> notification,
        CancellationToken ct)
    {
        var e = notification.DomainEvent;

        if (await _scheduleRepository.ExistsForMemberAsync(e.MemberId, ct))
            return;

        var schedule = Schedule.Create(e.MemberId);
        await _scheduleRepository.AddAsync(schedule, ct);
    }
}