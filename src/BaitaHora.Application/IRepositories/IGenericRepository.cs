using BaitaHora.Domain.Features.Common;

namespace BaitaHora.Application.IRepositories
{
    public interface IGenericRepository<T> where T : EntityBase
    {
        void MarkAsAdded(T entity);
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<T>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}