namespace BaitaHora.Infrastructure.Persistence.Interceptors;

using BaitaHora.Domain.Features.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class TimestampInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static void Stamp(DbContext? ctx)
    {
        if (ctx is null) return;
        var now = DateTimeOffset.UtcNow;

        foreach (var e in ctx.ChangeTracker.Entries<Entity>())
        {
            if (e.State == EntityState.Added)
            {
                e.Property(nameof(Entity.CreatedAtUtc)).CurrentValue = now;
                e.Property(nameof(Entity.UpdatedAtUtc)).CurrentValue = now;
            }
            else if (e.State == EntityState.Modified)
            {
                e.Property(nameof(Entity.CreatedAtUtc)).IsModified = false;
                e.Entity.Touch();
            }
        }
    }
}