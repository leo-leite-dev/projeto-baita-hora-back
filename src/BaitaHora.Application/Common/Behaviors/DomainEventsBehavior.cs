using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Features.Commons;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class DomainEventsBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly DbContext _db;
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsBehavior(DbContext db, IDomainEventDispatcher dispatcher)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var response = await next();

        var entries = _db.ChangeTracker.Entries<Entity>().ToList();
        var events = entries.SelectMany(e => e.Entity.DomainEvents).ToList();

        if (events.Count > 0)
            await _dispatcher.DispatchAsync(events, ct);

        return response;
    }
}