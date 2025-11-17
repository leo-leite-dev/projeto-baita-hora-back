using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public class CompanyServiceOfferingRepository
    : GenericRepository<CompanyServiceOffering>, ICompanyServiceOfferingRepository
{
    public CompanyServiceOfferingRepository(AppDbContext context) : base(context) { }

    public async Task<ServiceOfferingEditView?> GetByIdAsync(
        Guid companyId, Guid serviceOfferingId, CancellationToken ct)
    {
        return await _set
            .Where(s => s.CompanyId == companyId && s.Id == serviceOfferingId)
            .Select(s => new ServiceOfferingEditView
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price.Amount,
                Currency = s.Price.Currency
            })
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOfferingOptions>> ListServiceOfferingActiveForOptionsAsync(
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
            .Select(s => new ServiceOfferingOptions
            {
                Id = s.Id,
                Name = s.Name
            })
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOfferingDetails>> ListAllServicesByCompanyAsync(Guid companyId, CancellationToken ct)
    {
        return await _set
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .Select(s => new ServiceOfferingDetails
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price.Amount,
                Currency = s.Price.Currency,
                IsActive = s.IsActive,
                CreatedAtUtc = s.CreatedAtUtc,
                UpdatedAtUtc = s.UpdatedAtUtc
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOfferingOptions>> ListServiceOfferingActiveForMemberOptionsAsync(
        Guid companyId, Guid memberId, string? search, int take, CancellationToken ct)
    {
        IQueryable<CompanyServiceOffering> q = _set
            .Where(s => s.CompanyId == companyId && s.IsActive);

        var positions = _context.Set<CompanyPosition>();
        var members = _context.Set<CompanyMember>();

        q = q.Where(s =>
            positions
                .Where(p => p.CompanyId == companyId
                    && p.Members.Any(m => m.Id == memberId && m.CompanyId == companyId && m.IsActive))
                .Any(p => p.ServiceOfferings.Any(so => so.Id == s.Id)));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(s => EF.Functions.ILike(s.Name, $"%{term}%"));
        }

        return await q
            .OrderBy(s => s.Name)
            .Select(s => new ServiceOfferingOptions
            {
                Id = s.Id,
                Name = s.Name
            })
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}