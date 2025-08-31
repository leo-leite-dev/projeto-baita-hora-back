using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public interface IPositionGuards
{
    Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId);
    Task<Result> EnsureNoActiveMembersAsync(Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct);
}