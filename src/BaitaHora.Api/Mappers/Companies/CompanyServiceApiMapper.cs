using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Companies.Company.Patch;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyServiceOfferingsApiMappers
{
    public static CreateCompanyServiceOfferingCommand ToCommand(
        this CreateCompanyServiceOfferingRequest r, Guid companyId)
        => new CreateCompanyServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingName = r.ServiceOfferingName,
            Amount = r.Amount,
            Currency = r.Currency,
        };

    public static PatchCompanyServiceOfferingCommand ToCommand(
        this PatchCompanyServiceOfferingRequest r, Guid companyId, Guid serviceOfferingId)
        => new PatchCompanyServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingId = serviceOfferingId,
            ServiceOfferingName = r.ServiceOfferingName,
            Amount = r.Amount,
            Currency = r.Currency,
        };
}
