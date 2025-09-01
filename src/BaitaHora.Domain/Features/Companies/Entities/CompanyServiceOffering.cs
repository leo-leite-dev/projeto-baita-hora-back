using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Common.ValueObjects;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyServiceOffering : Entity
{
    public Guid CompanyId { get; private set; }
    public Money Price { get; private set; }

    private CompanyServiceOffering() { }

    public static CompanyServiceOffering Create(Guid companyId, string serviceName, Money price)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        var service = new CompanyServiceOffering
        {
            CompanyId = companyId,
            Name = NormalizeAndValidateName(serviceName),
            Price = price,
        };

        service.ValidatePriceInvariants();
        return service;
    }

    public bool Rename(string newName)
    {
        var normalized = NormalizeAndValidateName(newName);

        if (string.Equals(Name, normalized, StringComparison.OrdinalIgnoreCase))
            return false;

        Name = normalized;
        Touch();
        return true;
    }

    public void ChangePrice(Money newPrice)
    {
        Price = newPrice;

        ValidatePriceInvariants();
        Touch();
    }

    private void ValidatePriceInvariants()
    {
        if (Price.Amount <= 0)
            throw new CompanyException("Preço deve ser maior que zero.");
    }
}