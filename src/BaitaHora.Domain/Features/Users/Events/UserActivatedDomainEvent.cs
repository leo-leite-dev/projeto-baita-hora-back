using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Domain.Features.Users.Events;

public sealed record UserActivatedDomainEvent(Guid UserId) : IDomainEvent
{
    public DateTimeOffset OccurredOnUtc { get; } = DateTimeOffset.UtcNow;
}