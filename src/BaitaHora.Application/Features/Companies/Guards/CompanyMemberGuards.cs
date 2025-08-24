using BaitaHora.Application.Common;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public interface ICompanyMemberGuards
{
    Result<CompanyMember> GetMemberOrNotFound(Company company, Guid employeeId, bool requireActive = false);
}

public sealed class CompanyMemberGuards : ICompanyMemberGuards
{
    public Result<CompanyMember> GetMemberOrNotFound(Company company, Guid employeeId, bool requireActive = false)
    {
        var member = company.Members.SingleOrDefault(m => m.UserId == employeeId);
        if (member is null)
            return Result<CompanyMember>.NotFound("Funcionário não encontrado nesta empresa.");

        if (requireActive && !member.IsActive)
            return Result<CompanyMember>.BadRequest("O funcionário está inativo.");

        return Result<CompanyMember>.Ok(member);
    }
}