using System.Reflection;
using BaitaHora.Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task HandleAsync(T evt, CancellationToken ct);
}

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    public DomainEventDispatcher(IServiceProvider serviceProvider)
    => _serviceProvider = serviceProvider;

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var evt in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(evt.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public)!;
                var task = (Task)method.Invoke(handler, new object[] { evt, ct })!;
                await task;
            }
        }
    }
}