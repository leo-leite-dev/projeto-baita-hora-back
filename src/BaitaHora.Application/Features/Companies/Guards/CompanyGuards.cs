using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyGuards : ICompanyGuards
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyMemberRepository _memberRepository;
    private readonly ICompanyPermissionService _permissionService;

    public CompanyGuards(
        ICompanyRepository companyRepository,
        ICompanyMemberRepository memberRepository,
        ICompanyPermissionService permissionService)
    {
        _companyRepository = companyRepository;
        _memberRepository = memberRepository;
        _permissionService = permissionService;
    }

    public async Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdAsync(companyId, ct);
        return company is null
            ? Result<Company>.NotFound("Empresa não encontrada.")
            : Result<Company>.Ok(company);
    }

    public async Task<Result<Company>> GetWithServiceOfferingsOrNotFoundAsync(Guid companyId, CancellationToken ct)
    {
        var company = await _companyRepository.GetWithServiceOfferingsAsync(companyId, ct);
        return company is null
            ? Result<Company>.NotFound("Empresa não encontrada.")
            : Result<Company>.Ok(company);
    }

    public async Task<Result<CompanyMember>> GetActiveMembershipOrForbiddenAsync(
        Guid companyId, Guid userId, CancellationToken ct)
    {
        var member = await _memberRepository.GetByCompanyAndUserWithPositionAsync(companyId, userId, ct);

        if (member is null)
            return Result<CompanyMember>.Forbidden("Usuário não é membro da empresa.");

        if (!member.IsActive)
            return Result<CompanyMember>.Forbidden("Membro inativo nesta empresa.");

        return Result<CompanyMember>.Ok(member);
    }

    public async Task<Result<bool>> HasPermissionsOrForbiddenAsync(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct, bool requireAll = true)
    {
        var memRes = await GetActiveMembershipOrForbiddenAsync(companyId, userId, ct);
        if (memRes.IsFailure)
            return Result<bool>.FromError(memRes);

        var mask = await _permissionService.GetMaskAsync(companyId, userId, ct);
        if (mask is null)
            return Result<bool>.Forbidden("Usuário não é membro ativo da empresa.");

        bool ok = requireAll
            ? required.All(p => _permissionService.Has(mask.Value, p))
            : _permissionService.HasAny(mask.Value, required);

        return ok
            ? Result<bool>.Ok(true)
            : Result<bool>.Forbidden($"Permissão insuficiente. Requer: {string.Join(", ", required)}.");
    }

    public async Task<Result<CompanyRole>> GetUserRoleOrForbiddenAsync(
        Guid companyId, Guid userId, CancellationToken ct)
    {
        var memRes = await GetActiveMembershipOrForbiddenAsync(companyId, userId, ct);
        if (memRes.IsFailure)
            return Result<CompanyRole>.FromError(memRes);

        return Result<CompanyRole>.Ok(memRes.Value!.Role);
    }
}