namespace BaitaHora.Domain.Features.Common;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    void AddDomainEvent(DomainEvent domainEvent);
    void ClearDomainEvents();
}