using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Application.Features.Companies.ServiceOffering.Remove;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;
using BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

namespace BaitaHora.Api.Mappers.Companies;

public static class ServiceOfferingsApiMappers
{
    public static CreateServiceOfferingCommand ToCommand(this CreateServiceOfferingRequest r, Guid companyId)
        => new CreateServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingName = r.Name,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static PatchServiceOfferingCommand ToCommand(
        this PatchServiceOfferingRequest r,
        Guid serviceOfferingId,
        Guid companyId)
        => new PatchServiceOfferingCommand
        {
            ServiceOfferingId = serviceOfferingId,
            CompanyId = companyId,
            ServiceOfferingName = r.Name,
            Amount = r.Amount,
            Currency = r.Currency
        };

    public static RemoveServiceOfferingCommand ToCommand(Guid serviceOfferingId, Guid companyId)
        => new RemoveServiceOfferingCommand
        {
            ServiceOfferingId = serviceOfferingId,
            CompanyId = companyId
        };

    public static DisableServiceOfferingsCommand ToCommand(this DisableServiceOfferingsRequest r, Guid companyId)
        => new DisableServiceOfferingsCommand
        {
            CompanyId = companyId,
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ActivateServiceOfferingsCommand ToCommand(this ActivateServiceOfferingsRequest r, Guid companyId)
        => new ActivateServiceOfferingsCommand
        {
            CompanyId = companyId,
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };
}