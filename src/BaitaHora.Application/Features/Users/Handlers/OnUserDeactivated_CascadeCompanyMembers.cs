using BaitaHora.Domain.Features.Users.Events;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.Common.Events;

namespace BaitaHora.Application.Features.Users.Handlers;

public sealed class OnUserDeactivated_CascadeCompanyMembers : IDomainEventHandler<UserDeactivatedDomainEvent>
{
    private readonly ICompanyMemberRepository _memberRepository;

    public OnUserDeactivated_CascadeCompanyMembers(ICompanyMemberRepository memberRepository)
        => _memberRepository = memberRepository;

    public async Task HandleAsync(UserDeactivatedDomainEvent evt, CancellationToken ct)
    {
        var memberships = await _memberRepository.GetByUserIdAsync(evt.UserId, ct);
        foreach (var m in memberships)
            m.Deactivate();
    }
}