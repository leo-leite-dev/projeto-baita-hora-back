using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using DomainMoney = BaitaHora.Domain.Common.ValueObjects.Money;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateCompanyServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public CreateCompanyServiceOfferingUseCase(
        ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<CreateCompanyServiceOfferingResponse>> HandleAsync(
        CreateCompanyServiceOfferingCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.ExistsCompany(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<CreateCompanyServiceOfferingResponse>.FromError(compRes);

        var company = compRes.Value!;

        if (string.IsNullOrWhiteSpace(cmd.Currency))
            return Result<CreateCompanyServiceOfferingResponse>.BadRequest("Moeda é obrigatória.");

        var price = DomainMoney.Create(cmd.Amount, cmd.Currency!);

        var serviceOffering = company.AddServiceOffering(cmd.ServiceOfferingName, price);

        var response = new CreateCompanyServiceOfferingResponse(
            serviceOffering.Id,
            serviceOffering.ServiceOfferingName,
            serviceOffering.Price.Amount,
            serviceOffering.Price.Currency
        );

        return Result<CreateCompanyServiceOfferingResponse>.Created(response);
    }
}