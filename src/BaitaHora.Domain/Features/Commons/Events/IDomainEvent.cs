namespace BaitaHora.Domain.Common.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}