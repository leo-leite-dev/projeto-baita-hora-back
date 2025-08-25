using System.Reflection;
using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Common.Events;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Infrastructure.Common.Events;

public sealed class DomainEventAccessor : IDomainEventAccessor
{
    private readonly AppDbContext _db;

    public DomainEventAccessor(AppDbContext db) => _db = db;

    public IReadOnlyCollection<IDomainEvent> CollectDomainEventsAndClear()
    {
        var events = new List<IDomainEvent>();

        foreach (var entry in _db.ChangeTracker.Entries())
        {
            var entity = entry.Entity;
            if (entity is null) continue;

            if (entity is IHasDomainEvents has)
            {
                if (has.DomainEvents.Count > 0)
                {
                    events.AddRange(has.DomainEvents);
                    has.ClearDomainEvents();
                }
                continue;
            }

            var domProp = entity.GetType().GetProperty("DomainEvents", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var clearMeth = entity.GetType().GetMethod("ClearDomainEvents", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (domProp is null || clearMeth is null) continue;

            var value = domProp.GetValue(entity);
            if (value is IEnumerable<IDomainEvent> enumerable)
            {
                var collected = enumerable.ToList();
                if (collected.Count == 0) continue;

                events.AddRange(collected);
                clearMeth.Invoke(entity, null);
            }
        }

        return events;
    }
}