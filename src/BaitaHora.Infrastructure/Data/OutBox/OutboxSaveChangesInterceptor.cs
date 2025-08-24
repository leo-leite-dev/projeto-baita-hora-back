using Microsoft.EntityFrameworkCore.Diagnostics;
using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Infrastructure.Data.Outbox;

public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var db = eventData.Context!;
        var entries = db.ChangeTracker.Entries()
            .Where(e => e.Entity is IDomainEventsAccessor)
            .Select(e => (IDomainEventsAccessor)e.Entity)
            .ToList();

        var outbox = db.Set<OutboxMessage>();

        foreach (var agg in entries)
        {
            var events = agg.DequeueDomainEvents();
            foreach (var evt in events)
                outbox.Add(OutboxMessage.From(evt));
        }

        return await base.SavingChangesAsync(eventData, result, ct);
    }
}

public interface IDomainEventsAccessor
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
}