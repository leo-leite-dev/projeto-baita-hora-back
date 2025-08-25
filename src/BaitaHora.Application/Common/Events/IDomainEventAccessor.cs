using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Application.Common.Events;

public interface IDomainEventAccessor
{
    IReadOnlyCollection<IDomainEvent> CollectDomainEventsAndClear();
}