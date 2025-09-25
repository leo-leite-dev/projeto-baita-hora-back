using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
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

    public async Task<IReadOnlyList<PositionDetails>> ListAllPositionsByCompanyAsync(
        Guid companyId, CancellationToken ct)
    {
        return await _context.Positions
            .AsNoTracking()
            .Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.Name)
            .Select(p => new PositionDetails(
                p.Id,
                p.Name,
                p.AccessLevel,
                p.IsActive,
                p.CreatedAtUtc,
                p.UpdatedAtUtc,
                p.ServiceOfferings
                    .Select(s => new ServiceDto(s.Id, s.Name))
                    .ToList()
                    // p.Members
                    //     .Select(m => new MemberDto(
                    //         m.Id,
                    //         m.User.Username.Value, 
                    //         m.IsActive
                    //     ))
                    .ToList()
            ))
            .ToListAsync(ct);
    }
}