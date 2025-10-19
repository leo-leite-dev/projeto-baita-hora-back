using BaitaHora.Application.Companies.Features.Positions.Models;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public class CompanyPositionRepository : GenericRepository<CompanyPosition>, ICompanyPositionRepository
{
    public CompanyPositionRepository(AppDbContext context) : base(context) { }

    public async Task<PositionEditView?> GetByIdAsync(
        Guid companyId, Guid positionId, CancellationToken ct)
    {
        return await _context.Positions
            .AsNoTracking()
            .Where(p => p.CompanyId == companyId && p.Id == positionId)
            .Select(p => new PositionEditView
            {
                Id = p.Id,
                Name = p.Name,
                AccessLevel = p.AccessLevel,
                ServiceOfferings = p.ServiceOfferings
                    .Select(s => new ServiceDto { Id = s.Id, Name = s.Name })
                    .ToList()
            })
            .SingleOrDefaultAsync(ct);
    }
    
    public async Task<IReadOnlyList<PositionOptions>> ListActivePositionForOptionsAsync(
    Guid companyId, string? search, int take, CancellationToken ct)
    {
        var q = _set.Where(s => s.CompanyId == companyId && s.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(s => EF.Functions.ILike(s.Name, $"%{term}%"));
        }

        return await q
            .OrderBy(s => s.Name)
            .Select(s => new PositionOptions(s.Id, s.Name))
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PositionDetails>> ListAllPositionsByCompanyAsync(
        Guid companyId, CancellationToken ct)
    {
        return await _context.Positions
            .AsNoTracking()
            .Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.Name)
            .Select(p => new PositionDetails
            {
                Id = p.Id,
                Name = p.Name,
                IsActive = p.IsActive,
                CreatedAtUtc = p.CreatedAtUtc,
                UpdatedAtUtc = p.UpdatedAtUtc,
                ServiceOfferings = p.ServiceOfferings
                    .Select(s => new ServiceDto { Id = s.Id, Name = s.Name })
                    .ToList()
            })
            .ToListAsync(ct);
    }
}