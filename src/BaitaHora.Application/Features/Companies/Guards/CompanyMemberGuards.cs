using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyMemberGuards : ICompanyMemberGuards
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyMemberGuards(ICompanyRepository companyRepository)
        => _companyRepository = companyRepository;

    public Result<CompanyMember> ValidateMember( Company company, Guid memberId, bool requireActive)
    {
        var member = company.Members.SingleOrDefault(m => m.Id == memberId);

        if (member is null)
            return Result<CompanyMember>.NotFound("Funcionário não encontrado nesta empresa.");

        if (requireActive && !member.IsActive)
            return Result<CompanyMember>.BadRequest("O funcionário está inativo.");

        return Result<CompanyMember>.Ok(member);
    }

    public async Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForActivation(
        Guid companyId, IEnumerable<Guid>? employeeIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(employeeIds);
        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyMember>>.BadRequest("Nenhum funcionário informado.");

        var members = await _companyRepository.GetMembersByUserIdsAsync(companyId, ids, ct);

        var notFound = ids.Except(members.Select(m => m.UserId)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyMember>>.NotFound(
                $"Os funcionários {string.Join(", ", notFound)} não foram encontrados.");

        var inactive = members.Where(m => !m.IsActive).ToList();

        if (inactive.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyMember>>.Conflict(
                "Alguns funcionários já estão ativos e não podem ser reativados.");

        return Result<IReadOnlyCollection<CompanyMember>>.Ok(inactive);
    }

    public async Task<Result<IReadOnlyCollection<CompanyMember>>> ValidateMembersForDesactivation(
        Guid companyId, IEnumerable<Guid>? employeeIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(employeeIds);
        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyMember>>.BadRequest("Nenhum funcionário informado.");

        var members = await _companyRepository.GetMembersByUserIdsAsync(companyId, ids, ct);
        var found = members.Select(m => m.UserId).ToHashSet();

        var notFound = ids.Where(id => !found.Contains(id)).ToArray();
        if (notFound.Length > 0)
            return Result<IReadOnlyCollection<CompanyMember>>.NotFound(
                $"Funcionário(s) não encontrado(s) na empresa: {string.Join(", ", notFound)}");

        var active = members.Where(m => m.IsActive).ToList();
        if (active.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyMember>>.Conflict(
                "Alguns funcionários já estão inativos e não podem ser desativados novamente.");

        if (active.Any(m => m.Role == CompanyRole.Owner))
            return Result<IReadOnlyCollection<CompanyMember>>.Forbidden(
                "Não é permitido desativar o Founder/Owner.");

        return Result<IReadOnlyCollection<CompanyMember>>.Ok(active);
    }
}