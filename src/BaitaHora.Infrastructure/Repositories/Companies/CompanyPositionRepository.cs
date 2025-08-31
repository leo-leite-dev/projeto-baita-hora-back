using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public class CompanyPositionRepository : GenericRepository<CompanyPosition>, ICompanyPositionRepository
{
    public CompanyPositionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyCollection<Guid>> GetIdsWithActiveMembersAsync(Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct)
    {
        if (positionIds.Count == 0)
            return Array.Empty<Guid>();

        return await _context.Positions
            .Where(p => p.CompanyId == companyId && positionIds.Contains(p.Id))
            .Where(p => p.Members.Any(m => m.IsActive))
            .Select(p => p.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> HasActiveMembersAsync(Guid companyId, Guid positionId, CancellationToken ct)
    {
        return await _context.Positions
            .Where(p => p.CompanyId == companyId && p.Id == positionId)
            .AnyAsync(p => p.Members.Any(m => m.IsActive), ct);
    }
}