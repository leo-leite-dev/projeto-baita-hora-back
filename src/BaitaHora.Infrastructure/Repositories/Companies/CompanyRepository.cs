using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context) { }

    public async Task<Company?> GetWithServiceOfferingsAsync(Guid id, CancellationToken ct)
    {
        return await _context.Companies
            .AsTracking()
            .Where(c => c.Id == id)
            .Include(c => c.ServiceOfferings)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ct);
    }

    public async Task<Company?> GetWithPositionAndServiceOfferingsAsync(Guid id, CancellationToken ct)
    {
        return await _context.Companies
            .AsTracking()
            .Where(c => c.Id == id)
            .Include(c => c.Positions)
                .ThenInclude(p => p.ServiceOfferings)
            .Include(c => c.ServiceOfferings)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ct);
    }

    public async Task<Company?> GetByIdWithPositionsAndMembersAsync(Guid companyId, CancellationToken ct)
    {
        return await _context.Companies
            .AsTracking()
            .Include(c => c.Positions)
            .Include(c => c.Members)
            .AsSplitQuery()
            .SingleOrDefaultAsync(c => c.Id == companyId, ct);
    }

    public async Task<Company?> GetWithPositionsMembersAndServiceOfferingsAsync(
        Guid companyId, CancellationToken ct)
    {
        return await _context.Companies
            .AsTracking()
            .Where(c => c.Id == companyId)
            .Include(c => c.Positions)
                .ThenInclude(p => p.ServiceOfferings)
            .Include(c => c.Positions)
                .ThenInclude(p => p.Members)
            .Include(c => c.ServiceOfferings)
            .Include(c => c.Members)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ct);
    }

    public async Task AddImageAsync(CompanyImage image, CancellationToken ct = default)
    {
        await _context.Set<CompanyImage>().AddAsync(image, ct);
    }

    public async Task<List<CompanyMember>> GetMembersByUserIdsAsync(
        Guid companyId, IReadOnlyCollection<Guid> userIds, CancellationToken ct = default)
    {
        if (userIds is null || userIds.Count == 0)
            return new List<CompanyMember>();

        return await _context.Members
            .Where(m => m.CompanyId == companyId && userIds.Contains(m.UserId))
            .ToListAsync(ct);
    }

    public async Task<List<CompanyPosition>> GetPositionsByIdsAsync(
        Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct = default)
    {
        if (positionIds is null || positionIds.Count == 0)
            return new List<CompanyPosition>();

        return await _context.Positions
            .Where(p => p.CompanyId == companyId && positionIds.Contains(p.Id))
            .ToListAsync(ct);
    }

    public async Task<List<CompanyServiceOffering>> GetServiceOfferingsByIdsAsync(
        Guid companyId, IReadOnlyCollection<Guid> ids, CancellationToken ct = default)
    {
        if (ids is null || ids.Count == 0)
            return new List<CompanyServiceOffering>();

        return await _context.ServiceOfferings
            .Where(s => s.CompanyId == companyId && ids.Contains(s.Id))
            .ToListAsync(ct);
    }
}