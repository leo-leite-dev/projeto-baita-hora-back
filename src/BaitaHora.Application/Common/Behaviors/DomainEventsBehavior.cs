// BaitaHora.Application/Common/Behaviors/DomainEventsBehavior.cs
using BaitaHora.Application.Common.Events;
using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class DomainEventsBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IDomainEventAccessor _accessor;
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsBehavior(
        IDomainEventAccessor accessor,
        IDomainEventDispatcher dispatcher)
    {
        _accessor = accessor;
        _dispatcher = dispatcher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();

        var events = _accessor.CollectDomainEventsAndClear();
        if (events.Count > 0)
            await _dispatcher.DispatchAsync(events, ct);

        return response;
    }
}