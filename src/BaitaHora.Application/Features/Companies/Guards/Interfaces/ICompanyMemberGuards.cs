using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyMemberGuards
{
    Result<CompanyMember> ValidateMember(Company company, Guid memberId, bool requireActive);
    Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForActivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForDesactivation(Guid companyId, IEnumerable<Guid>? employeeIds, CancellationToken ct);
}