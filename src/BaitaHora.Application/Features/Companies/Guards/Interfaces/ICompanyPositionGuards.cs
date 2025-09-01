using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public interface ICompanyPositionGuards
{
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForActivation(IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForDeactivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
    Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId);
}