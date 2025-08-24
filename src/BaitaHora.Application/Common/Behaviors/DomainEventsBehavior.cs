using BaitaHora.Application.Abstractions.Data;
using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Features.Common;
using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class DomainEventsBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IAppDbContext _db;
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsBehavior(IAppDbContext db, IDomainEventDispatcher dispatcher)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();

        var entitiesWithEvents = _db.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        if (entitiesWithEvents.Count == 0)
            return response;

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var ev in domainEvents)
            await _dispatcher.PublishAsync(ev, ct);

        return response;
    }
}