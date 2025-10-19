using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }

    public CompanyPermission PermissionMask { get; private set; }
    public CompanyRole AccessLevel { get; private set; }
    public bool IsSystem { get; private set; }

    private readonly List<CompanyServiceOffering> _serviceOfferings = new();
    public IReadOnlyCollection<CompanyServiceOffering> ServiceOfferings => _serviceOfferings.AsReadOnly();
    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

    private CompanyPosition() { }

    internal static CompanyPosition Create(
        Guid companyId,
        string positionName,
        CompanyRole accessLevel,
        bool isSystem = false,
        CompanyPermission permissionMask = CompanyPermission.None)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (accessLevel == CompanyRole.Owner && !isSystem)
            throw new CompanyException("Cargo 'Owner' não existe; Owner apenas para o fundador (sistema).");

        return new CompanyPosition
        {
            CompanyId = companyId,
            Name = NormalizeAndValidateName(positionName),
            AccessLevel = accessLevel,
            IsSystem = isSystem,
            PermissionMask = permissionMask
        };
    }

    internal void AddServiceOffering(CompanyServiceOffering service)
    {
        if (service is null)
            throw new CompanyException("Serviço inválido.");

        if (service.CompanyId != CompanyId)
            throw new CompanyException("Serviço pertence a outra empresa.");

        if (!service.IsActive)
            throw new CompanyException("Serviço inativo.");

        if (_serviceOfferings.Any(s => s.Id == service.Id))
            return;

        _serviceOfferings.Add(service);
    }

    internal void AddServiceOfferings(IEnumerable<CompanyServiceOffering>? services)
    {
        if (services is null)
            return;

        foreach (var s in services)
            AddServiceOffering(s);
    }

    internal bool Rename(string newName)
    {
        var normalized = NormalizeAndValidateName(newName);

        var isFounderName = string.Equals(normalized, "Fundador", StringComparison.OrdinalIgnoreCase);
        var isFounderSystem = IsSystem && AccessLevel == CompanyRole.Owner;

        if (isFounderName && !isFounderSystem)
            throw new CompanyException("Cargo 'Fundador' só é permitido para a posição de sistema do fundador.");

        if (NameEquals(Name, normalized))
            return false;

        Name = normalized;

        Touch();
        return true;
    }

    internal void ChangeAccessLevel(CompanyRole newLevel)
    {

        if (!Enum.IsDefined(typeof(CompanyRole), newLevel) || newLevel == CompanyRole.Unknown)
            throw new CompanyException("Nível de acesso inválido.");

        if (newLevel == CompanyRole.Owner && !IsSystem)
            throw new CompanyException("Nível Owner só é permitido para a posição de sistema do fundador.");

        AccessLevel = newLevel;
        Touch();
    }

    internal void ReplaceServiceOfferings(IEnumerable<CompanyServiceOffering> services)
    {
        if (services is null)
            throw new CompanyException("Serviços inválidos.");

        _serviceOfferings.Clear();
        AddServiceOfferings(services);

        Touch();
    }

    internal void RemoveServiceOfferings(IReadOnlyCollection<Guid> serviceOfferingIds)
    {
        if (serviceOfferingIds is null || serviceOfferingIds.Count == 0)
            throw new CompanyException("Seleção de serviços inválida.");

        var ids = serviceOfferingIds.Where(x => x != Guid.Empty).Distinct().ToArray();
        if (ids.Length == 0)
            throw new CompanyException("Seleção de serviços inválida.");

        var linked = _serviceOfferings.Select(s => s.Id).ToHashSet();
        var toRemove = ids.Where(linked.Contains).ToArray();
        if (toRemove.Length == 0)
            throw new CompanyException("Nenhum dos serviços informados está associado ao cargo.");

        if (_serviceOfferings.Count - toRemove.Length < 1)
            throw new CompanyException("Um cargo deve ter ao menos um serviço associado.");

        var toRemoveSet = toRemove.ToHashSet();
        _serviceOfferings.RemoveAll(s => toRemoveSet.Contains(s.Id));
        Touch();
    }

    public void SetPermissionMask(CompanyPermission newMask)
    {
        if (IsSystem && AccessLevel == CompanyRole.Owner)
        {
            PermissionMask = CompanyPermission.All;
            return;
        }

        PermissionMask = newMask;
    }
}