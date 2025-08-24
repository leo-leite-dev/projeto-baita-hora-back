
using BaitaHora.Domain.Features.Common;

namespace BaitaHora.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task PublishAsync(DomainEvent domainEvent, CancellationToken ct = default);
    Task PublishAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken ct = default);
}