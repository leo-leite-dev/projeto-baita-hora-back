using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public class CompanyServiceOfferingRepository
    : GenericRepository<CompanyServiceOffering>, ICompanyServiceOfferingRepository
{
    public CompanyServiceOfferingRepository(AppDbContext context) : base(context) { }

    public async Task<ServiceOfferingDetails?> GetByIdAsync(
        Guid companyId, Guid serviceOfferingId, CancellationToken ct)
    {
        return await _set
            .Where(s => s.CompanyId == companyId && s.Id == serviceOfferingId)
            .Select(s => new ServiceOfferingDetails(
                s.Id,
                s.Name,
                s.Price.Amount,
                s.Price.Currency,
                s.IsActive,
                s.CreatedAtUtc))
            .AsNoTracking()
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOfferingComboItem>> ListActiveForComboAsync(
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
            .Select(s => new ServiceOfferingComboItem(s.Id, s.Name))
            .Take(take)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOfferingDetails>> ListAllByCompanyAsync(Guid companyId, CancellationToken ct)
    {
        return await _set
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .Select(s => new ServiceOfferingDetails(
                s.Id,
                s.Name,
                s.Price.Amount,
                s.Price.Currency,
                s.IsActive,
                s.CreatedAtUtc))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}