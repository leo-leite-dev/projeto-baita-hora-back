using BaitaHora.Application.Companies.Features.Positions.Models;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyPositionRepository : IGenericRepository<CompanyPosition>
{
    Task<PositionEditView?> GetByPositionIdAsync(Guid companyId, Guid positionId, CancellationToken ct);
    Task<IReadOnlyList<PositionOptions>> ListActivePositionForOptionsAsync(Guid companyId, string? search, int take, CancellationToken ct);
    Task<IReadOnlyList<PositionDetails>> ListAllPositionsByCompanyAsync(Guid companyId, CancellationToken ct);
}