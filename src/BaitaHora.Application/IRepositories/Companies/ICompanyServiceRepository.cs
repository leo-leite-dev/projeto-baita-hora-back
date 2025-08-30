using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface IServiceOfferingRepository : IGenericRepository<ServiceOffering>
{
    // Task<ServiceOffering?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceOffering>> ListByCompanyAsync(Guid companyId, CancellationToken ct = default);
    // Task<IReadOnlyList<ServiceOffering>> ListActiveByPositionAsync(Guid positionId, CancellationToken ct = default);
    // Task<bool> IsServiceLinkedToPositionAsync(Guid serviceId, Guid positionId, CancellationToken ct = default);
}