using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Domain.Features.Companies.Events;

public sealed record CompanyMemberCreatedDomainEvent(Guid MemberId) : IDomainEvent
{
    public DateTimeOffset OccurredOnUtc { get; } = DateTimeOffset.UtcNow;
}
