using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Application.Features.Companies.ServiceOffering.Remove;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;
using BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

namespace BaitaHora.Api.Mappers.Companies;

public static class ServiceOfferingsApiMappers
{
    public static CreateServiceOfferingCommand ToCommand(
        this CreateServiceOfferingRequest r)
        => new CreateServiceOfferingCommand
        {
            ServiceOfferingName = r.Name,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static PatchServiceOfferingCommand ToCommand(
        this PatchServiceOfferingRequest r, Guid serviceOfferingId)
        => new PatchServiceOfferingCommand
        {
            ServiceOfferingId = serviceOfferingId,
            ServiceOfferingName = r.Name,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static RemoveServiceOfferingCommand ToCommand(Guid serviceOfferingId)
        => new RemoveServiceOfferingCommand
        {
            ServiceOfferingId = serviceOfferingId
        };

    public static DisableServiceOfferingsCommand ToCommand(
        this DisableServiceOfferingsRequest r)
        => new DisableServiceOfferingsCommand
        {
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ActivateServiceOfferingsCommand ToCommand(
        this ActivateServiceOfferingsRequest r)
        => new()
        {
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };
}