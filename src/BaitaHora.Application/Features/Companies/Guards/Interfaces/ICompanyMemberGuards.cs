using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyMemberGuards
{
    Task<Result<CompanyMember>> EnsureMemberExistsAsync(Guid companyId, Guid employeeId, bool requireActive, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForActivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForDeactivation(Guid companyId, IEnumerable<Guid>? employeeIds, CancellationToken ct);
}