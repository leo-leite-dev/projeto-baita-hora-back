using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Companies.Policies;
using BaitaHora.Domain.Features.Companies.Entities;
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

    public async Task<Result<Company>> EnsureCompanyExists(Guid companyId, CancellationToken ct)
    {
        if (companyId == Guid.Empty)
            return Result<Company>.BadRequest("CompanyId inválido.");

        var company = await _companyRepository.GetByIdAsync(companyId, ct);
        return company is null
            ? Result<Company>.NotFound("Empresa não encontrada.")
            : Result<Company>.Ok(company);
    }

    private async Task<Result<Company>> GetCompanyWithAsync(Guid companyId, CancellationToken ct, Func<Guid, CancellationToken, Task<Company?>> loader)
    {
        var ensure = await EnsureCompanyExists(companyId, ct);
        if (!ensure.IsSuccess)
            return ensure;

        var company = await loader(companyId, ct);
        if (company is null)
            return Result<Company>.NotFound("Empresa não encontrada.");

        return Result<Company>.Ok(company);
    }

    public Task<Result<Company>> GetWithServiceOfferings(Guid companyId, CancellationToken ct)
        => GetCompanyWithAsync(companyId, ct, _companyRepository.GetWithServiceOfferingsAsync);

    public Task<Result<Company>> GetWithPositionsAndServiceOfferings(Guid companyId, CancellationToken ct)
        => GetCompanyWithAsync(companyId, ct, _companyRepository.GetWithPositionAndServiceOfferingsAsync);

    public Task<Result<Company>> GetWithPositionsAndMembers(Guid companyId, CancellationToken ct)
        => GetCompanyWithAsync(companyId, ct, _companyRepository.GetByIdWithPositionsAndMembersAsync);

    public Task<Result<Company>> GetWithPositionsMembersAndServiceOfferings(Guid companyId, CancellationToken ct)
        => GetCompanyWithAsync(companyId, ct, _companyRepository.GetWithPositionsMembersAndServiceOfferingsAsync);

    private async Task<Result<CompanyMember>> EnsureActiveMembershipByMemberId(Guid companyId, Guid memberId, CancellationToken ct)
    {
        if (companyId == Guid.Empty)
            return Result<CompanyMember>.BadRequest("CompanyId inválido.");

        if (memberId == Guid.Empty)
            return Result<CompanyMember>.BadRequest("MemberId inválido.");

        var member = await _memberRepository.GetByIdWithPositionAsync(companyId, memberId, ct);
        if (member is null)
            return Result<CompanyMember>.Forbidden("Membro não encontrado.");

        if (member.CompanyId != companyId)
            return Result<CompanyMember>.Forbidden("Membro não pertence a esta empresa.");

        if (!member.IsActive)
            return Result<CompanyMember>.Forbidden("Membro inativo nesta empresa.");

        return Result<CompanyMember>.Ok(member);
    }

    public Task<Result<CompanyMember>> GetActiveMembership(Guid companyId, Guid memberId, CancellationToken ct)
        => EnsureActiveMembershipByMemberId(companyId, memberId, ct);

    public async Task<Result<bool>> HasPermissions(Guid companyId, Guid memberId, IEnumerable<CompanyPermission> required, CancellationToken ct, bool requireAll = true)
    {
        if (required is null)
            return Result<bool>.BadRequest("Conjunto de permissões requerido é nulo.");

        var memRes = await EnsureActiveMembershipByMemberId(companyId, memberId, ct);
        if (memRes.IsFailure)
            return Result<bool>.FromError(memRes);

        var membership = memRes.Value!;
        var userId = membership.UserId;

        var baseMask = await _permissionService.GetMaskAsync(companyId, userId, ct);
        if (baseMask is null)
            return Result<bool>.Forbidden("Usuário não é membro ativo da empresa.");

        var roleMask = CompanyRolePolicies.GetPermissions(membership.Role);
        var effective = baseMask.Value | roleMask;
        var requiredList = (required as IList<CompanyPermission>) ?? required.ToList();

        bool ok = requireAll
            ? requiredList.All(p => _permissionService.Has(effective, p))
            : _permissionService.HasAny(effective, requiredList);

        return ok
            ? Result<bool>.Ok(true)
            : Result<bool>.Forbidden($"Permissão insuficiente. Requer: {string.Join(", ", requiredList)}.");
    }
}