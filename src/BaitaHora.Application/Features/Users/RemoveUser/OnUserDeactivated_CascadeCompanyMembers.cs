using BaitaHora.Application.Common.Events;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Users.Events;
using MediatR;

namespace BaitaHora.Application.Features.Users.RemoveUser;

public sealed class OnUserDeactivated_CascadeCompanyMembers
    : INotificationHandler<DomainEventNotification<UserDeactivatedDomainEvent>>
{
    private readonly ICompanyMemberRepository _memberRepository;

    public OnUserDeactivated_CascadeCompanyMembers(ICompanyMemberRepository companyMemberRepository)
        => _memberRepository = companyMemberRepository;

    public async Task Handle(DomainEventNotification<UserDeactivatedDomainEvent> notification, CancellationToken ct)
    {
        var evt = notification.DomainEvent;

        var memberships = await _memberRepository.GetByUserIdAsync(evt.UserId, ct);
        foreach (var m in memberships)
            m.Deactivate();
    }
}