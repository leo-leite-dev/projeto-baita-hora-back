using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Features.Common;
using MediatR;

namespace BaitaHora.Infrastructure.Common.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher) => _publisher = publisher;

    public Task PublishAsync(DomainEvent domainEvent, CancellationToken ct = default)
        => _publisher.Publish(domainEvent, ct);

    public async Task PublishAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken ct = default)
    {
        foreach (var ev in domainEvents)
            await _publisher.Publish(ev, ct);
    }
}