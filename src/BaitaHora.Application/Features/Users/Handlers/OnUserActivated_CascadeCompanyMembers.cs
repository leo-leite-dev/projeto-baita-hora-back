using BaitaHora.Application.Common.Events;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Users.Events;

namespace BaitaHora.Application.Features.Users.Handlers;

public sealed class OnUserActivated_CascadeCompanyMembers : IDomainEventHandler<UserActivatedDomainEvent>
{
    private readonly ICompanyMemberRepository _memberRepository;

    public OnUserActivated_CascadeCompanyMembers(ICompanyMemberRepository memberRepository)
        => _memberRepository = memberRepository;

    public async Task HandleAsync(UserActivatedDomainEvent evt, CancellationToken ct)
    {
        var memberships = await _memberRepository.GetByUserIdAsync(evt.UserId, ct);
        foreach (var m in memberships)
            m.Activate();
    }
}