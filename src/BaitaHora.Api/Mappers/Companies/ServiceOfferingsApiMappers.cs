using BaitaHora.Application.Features.Companies.ServiceOffering.Activate;
using BaitaHora.Application.Features.Companies.ServiceOffering.Disable;
using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Application.Features.Companies.ServiceOffering.Remove;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Companies.Company.Patch;

namespace BaitaHora.Api.Mappers.Companies;

public static class ServiceOfferingsApiMappers
{
    public static CreateServiceOfferingCommand ToCommand(
        this CreateServiceOfferingRequest r, Guid companyId)
        => new CreateServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingName = r.ServiceOfferingName,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static PatchServiceOfferingCommand ToCommand(
        this PatchServiceOfferingRequest r, Guid companyId, Guid serviceOfferingId)
        => new PatchServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingId = serviceOfferingId,
            ServiceOfferingName = r.ServiceOfferingName,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static RemoveServiceOfferingCommand ToCommand(
        Guid companyId, Guid serviceOfferingId)
        => new RemoveServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingId = serviceOfferingId
        };

    public static DisableServiceOfferingCommand ToDisableCommand(
        Guid companyId, Guid serviceOfferingId)
        => new DisableServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingId = serviceOfferingId
        };

    public static ActivateServiceOfferingCommand ToActivateCommand(
        Guid companyId, Guid serviceOfferingId)
        => new()
        {
            CompanyId = companyId,
            ServiceOfferingId = serviceOfferingId
        };
}