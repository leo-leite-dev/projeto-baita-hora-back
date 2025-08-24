using BaitaHora.Domain.Common.Events;

public sealed record UserRegistered(Guid UserId) : IDomainEvent
{
    public DateTimeOffset OccurredOnUtc { get; } = DateTimeOffset.UtcNow;
}