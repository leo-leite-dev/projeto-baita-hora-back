using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Common.ValueObjects;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyServiceOffering : Entity
{
    public Guid CompanyId { get; private set; }

    public string ServiceOfferingName { get; private set; } = null!;
    public Money Price { get; private set; }

    private CompanyServiceOffering() { }

    public static CompanyServiceOffering Create(Guid companyId, string serviceOfferingName, Money price)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (string.IsNullOrWhiteSpace(serviceOfferingName))
            throw new CompanyException("Nome do serviço é obrigatório.");

        if (price.Amount <= 0)
            throw new CompanyException("Preço deve ser maior que zero.");

        var serviceOfferig = new CompanyServiceOffering
        {
            CompanyId = companyId,
            ServiceOfferingName = ActivatableEntity.NormalizeSpaces(serviceOfferingName),
            Price = price,
        };

        return serviceOfferig;
    }

    public bool Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new CompanyException("Nome do serviço é obrigatório.");

        var normalized = ActivatableEntity.NormalizeSpaces(newName);
        if (ServiceOfferingName == normalized) return false;

        ServiceOfferingName = normalized;
        Touch();
        return true;
    }

    public void ChangePrice(Money newPrice)
    {
        Price = newPrice;
        Touch();
    }
}