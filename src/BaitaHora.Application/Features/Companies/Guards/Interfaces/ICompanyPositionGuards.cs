using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyPositionGuards
{
    Result<CompanyPosition> ValidatePosition(Company company, Guid positionId, bool requireActive);
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForActivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForDesactivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
}