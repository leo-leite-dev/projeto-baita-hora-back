using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyPositionGuards
{
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForActivation(IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForDeactivation(IEnumerable<Guid>? positionIds, CancellationToken ct);
    Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId);
}