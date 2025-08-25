using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Common.Events;
using MediatR;

namespace BaitaHora.Infrastructure.Common.Events;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher) 
    {
        _publisher = publisher;
    }

    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
    {
        var tasks = new List<Task>();

        foreach (var ev in domainEvents)
        {
            var evType   = ev.GetType();
            var notifTyp = typeof(DomainEventNotification<>).MakeGenericType(evType);

            var notif = Activator.CreateInstance(notifTyp, ev)
                       ?? throw new InvalidOperationException($"Não foi possível instanciar {notifTyp.Name}");

            tasks.Add(_publisher.Publish((INotification)notif, ct));
        }

        return Task.WhenAll(tasks);
    }
}