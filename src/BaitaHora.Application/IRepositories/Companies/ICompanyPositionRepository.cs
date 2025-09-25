using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyPositionRepository : IGenericRepository<CompanyPosition>
{
    Task<IReadOnlyCollection<Guid>> GetIdsWithActiveMembersAsync(Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct);
    Task<bool> HasActiveMembersAsync(Guid companyId, Guid positionId, CancellationToken ct);
    Task<IReadOnlyList<PositionDetails>> ListAllPositionsByCompanyAsync(Guid companyId, CancellationToken ct);
}