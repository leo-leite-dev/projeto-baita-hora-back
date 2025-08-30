using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories;

public sealed class ServiceOfferingRepository : GenericRepository<ServiceOffering>, IServiceOfferingRepository
{
    public ServiceOfferingRepository(AppDbContext context) : base(context) { }

    // public Task<ServiceOffering?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
    //     => _set.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && s.IsActive, ct);

    public async Task<IReadOnlyList<ServiceOffering>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default)
        => await _set.AsNoTracking()
            .Where(s => s.CompanyId == companyId)
            .ToListAsync(ct);

    // public async Task<IReadOnlyList<ServiceOffering>> ListActiveByPositionAsync(Guid positionId, CancellationToken ct = default)
    //     => await _set.AsNoTracking()
    //         .Where(s => s.IsActive && s.PositionLinks.Any(l => l.PositionId == positionId))
    //         .ToListAsync(ct);

    // public Task<bool> IsServiceOfferingLinkedToPositionAsync(Guid ServiceOfferingId, Guid positionId, CancellationToken ct = default)
    //     => _set.AsNoTracking()
    //         .AnyAsync(s => s.Id == ServiceOfferingId && s.IsActive && s.PositionLinks.Any(l => l.PositionId == positionId), ct);
}