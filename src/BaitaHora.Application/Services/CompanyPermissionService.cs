using BaitaHora.Application.Common.Caching;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

public sealed class CompanyPermissionService : ICompanyPermissionService
{
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly PermissionCache _cache;

    public CompanyPermissionService(ICompanyMemberRepository companyMemberRepository, PermissionCache cache)
        => (_companyMemberRepository, _cache) = (companyMemberRepository, cache);

    public async Task<CompanyPermission?> GetMaskAsync(Guid companyId, Guid userId, CancellationToken ct)
    {
        if (_cache.TryGet(companyId, userId, out var cached))
            return cached;

        var member = await _companyMemberRepository.GetByCompanyAndUserWithPositionAsync(companyId, userId, ct);
        if (member is null || !member.IsActive)
        {
            _cache.Set(companyId, userId, null);
            return null;
        }

        if (member.Role == CompanyRole.Owner)
        {
            _cache.Set(companyId, userId, CompanyPermission.All);
            return CompanyPermission.All;
        }

        CompanyPermission mask = CompanyPermission.None;

        if (member.PrimaryPosition is not null)
            mask |= member.PrimaryPosition.PermissionMask;

        mask |= member.DirectPermissionMask;

        _cache.Set(companyId, userId, mask);
        return mask;
    }

    public bool Has(CompanyPermission mask, CompanyPermission required)
        => (mask & required) == required;

    public bool HasAny(CompanyPermission mask, IEnumerable<CompanyPermission> required)
    {
        foreach (var r in required)
            if ((mask & r) == r) return true;
        return false;
    }

    public async Task<bool> CanAsync(Guid companyId, Guid userId, CompanyPermission required, CancellationToken ct)
    {
        var mask = await GetMaskAsync(companyId, userId, ct);
        if (mask is null) return false;
        return Has(mask.Value, required);
    }

    public async Task<bool> CanAnyAsync(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct)
    {
        var mask = await GetMaskAsync(companyId, userId, ct);
        if (mask is null) return false;
        return HasAny(mask.Value, required);
    }
}