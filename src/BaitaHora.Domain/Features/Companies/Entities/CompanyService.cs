using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyService : Entity
{
    public Guid CompanyId { get; private set; }

    public string ServiceName { get; private set; } = null!;
    public Money Price { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<CompanyPositionService> _positionLinks = new();
    public IReadOnlyCollection<CompanyPositionService> PositionLinks => _positionLinks.AsReadOnly();

    private CompanyService() { }

    public static CompanyService Create(Guid companyId, string serviceName, Money price)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("Empresa inválida.");

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new CompanyException("Nome do serviço é obrigatório.");

        var service = new CompanyService
        {
            CompanyId = companyId,
            IsActive = true
        };

        service.SetName(serviceName);
        service.SetPrice(price);
        return service;
    }

    public bool SetName(string newName)
    {
        var normalized = newName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new CompanyException("Nome do serviço é obrigatório.");

        if (string.Equals(ServiceName, normalized, StringComparison.Ordinal))
            return false;

        ServiceName = normalized;
        return true;
    }

    // public bool Rename(string name)
    // {
    //     var n = name?.Trim();
    //     if (string.IsNullOrWhiteSpace(n)) throw new CompanyException("Nome obrigatório.");
    //     if (string.Equals(Name, n, StringComparison.Ordinal)) return false;
    //     Name = n!;

    //     return true;
    // }

    public bool SetPrice(Money price)
    {
        if (Price == price)
            return false;

        Price = price;
        return true;
    }

    public bool AddPosition(CompanyPosition position)
    {
        if (position is null)
            throw new CompanyException("Cargo inválido.");

        if (position.CompanyId != CompanyId)
            throw new CompanyException("Cargo pertence a outra empresa.");

        if (!position.IsActive)
            throw new CompanyException("Cargo inativo. Não é possível vincular ao serviço.");

        if (_positionLinks.Any(l => l.PositionId == position.Id))
            return false;

        _positionLinks.Add(CompanyPositionService.Link(this, position));
        Touch();
        return true;
    }

    public int SetPositions(IEnumerable<CompanyPosition> positions)
    {
        if (positions is null)
            throw new CompanyException("Lista de cargos inválida.");

        var list = positions.ToList();

        foreach (var p in list)
        {
            if (p is null)
                throw new CompanyException("Lista contém cargo inválido.");

            if (p.CompanyId != CompanyId)
                throw new CompanyException("Há cargo de outra empresa na lista.");

            if (!p.IsActive)
                throw new CompanyException($"Cargo '{p.PositionName}' está inativo.");
        }

        var desiredIds = list.Select(p => p.Id).ToHashSet();

        var removed = _positionLinks.RemoveAll(l => !desiredIds.Contains(l.PositionId));

        var added = 0;
        foreach (var p in list)
            if (AddPosition(p)) added++;

        if (removed > 0 || added > 0) Touch();

        return added;
    }
}