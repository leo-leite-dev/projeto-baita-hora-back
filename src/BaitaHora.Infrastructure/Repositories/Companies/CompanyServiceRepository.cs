using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories;

public sealed class CompanyServiceRepository : GenericRepository<CompanyService>, ICompanyServiceRepository
{
    public CompanyServiceRepository(AppDbContext context) : base(context) { }

    public Task<CompanyService?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
        => _set.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.IsActive, ct);

    public async Task<IReadOnlyList<CompanyService>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default)
        => await _set.AsNoTracking()
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CompanyService>> ListActiveByPositionAsync(Guid positionId, CancellationToken ct = default)
        => await _set.AsNoTracking()
            .Where(s => s.IsActive && s.PositionLinks.Any(l => l.PositionId == positionId))
            .ToListAsync(ct);

    public Task<bool> IsServiceLinkedToPositionAsync(Guid serviceId, Guid positionId, CancellationToken ct = default)
        => _set.AsNoTracking()
            .AnyAsync(s => s.Id == serviceId && s.IsActive && s.PositionLinks.Any(l => l.PositionId == positionId), ct);
}