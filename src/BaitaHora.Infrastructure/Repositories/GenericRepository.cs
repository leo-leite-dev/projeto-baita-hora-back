using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _set;

    public GenericRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _set = _context.Set<T>();
    }

    public void MarkAsAdded(T entity)
        => _context.Entry(entity).State = EntityState.Added;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _set.FindAsync(new object?[] { id }, ct).AsTask();

    public Task<List<T>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct = default)
    {
        if (ids is null || ids.Count == 0)
            return Task.FromResult(new List<T>());

        return _set
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        await _set.AddAsync(entity, ct);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null)
         throw new ArgumentNullException(nameof(entity));

        entity.Touch();

        var entry = _context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _set.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => _set.AsNoTracking().ToListAsync(ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _set.AnyAsync(e => e.Id == id, ct);
}