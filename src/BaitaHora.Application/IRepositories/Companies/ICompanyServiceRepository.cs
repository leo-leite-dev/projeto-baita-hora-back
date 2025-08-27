using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyServiceRepository : IGenericRepository<CompanyService>
{
    Task<CompanyService?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyService>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyService>> ListActiveByPositionAsync(Guid positionId, CancellationToken ct = default);
    Task<bool> IsServiceLinkedToPositionAsync(Guid serviceId, Guid positionId, CancellationToken ct = default);
}