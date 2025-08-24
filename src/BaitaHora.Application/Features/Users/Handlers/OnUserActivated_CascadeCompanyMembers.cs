using BaitaHora.Application.Common.Events;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Users.Events;
using MediatR;

namespace BaitaHora.Application.Features.Users.Handlers;

public sealed class OnUserActivated_CascadeCompanyMembers
    : INotificationHandler<DomainEventNotification<UserActivatedDomainEvent>>
{
    private readonly ICompanyMemberRepository _memberRepository;

    public OnUserActivated_CascadeCompanyMembers(ICompanyMemberRepository companyMemberRepository)
        => _memberRepository = companyMemberRepository;

    public async Task Handle(DomainEventNotification<UserActivatedDomainEvent> notification, CancellationToken ct)
    {
        var evt = notification.DomainEvent;

        var memberships = await _memberRepository.GetByUserIdAsync(evt.UserId, ct);
        foreach (var m in memberships)
            m.Activate();

    }
}