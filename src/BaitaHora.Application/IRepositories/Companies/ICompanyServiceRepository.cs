using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyServiceOfferingRepository : IGenericRepository<CompanyServiceOffering>
{
    // Task<CompanyServiceOffering?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyServiceOffering>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default);
    // Task<IReadOnlyList<CompanyServiceOffering>> ListActiveByPositionAsync(Guid positionId, CancellationToken ct = default);
    // Task<bool> IsServiceLinkedToPositionAsync(Guid serviceId, Guid positionId, CancellationToken ct = default);
}