using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using DomainMoney = BaitaHora.Domain.Common.ValueObjects.Money;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public CreateServiceOfferingUseCase(
        ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<CreateServiceOfferingResponse>> HandleAsync(
        CreateServiceOfferingCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<CreateServiceOfferingResponse>.FromError(compRes);

        var company = compRes.Value!;

        if (string.IsNullOrWhiteSpace(cmd.Currency))
            return Result<CreateServiceOfferingResponse>.BadRequest("Moeda é obrigatória.");

        var price = DomainMoney.RequirePositive(cmd.Amount, cmd.Currency!);

        var serviceOffering = company.AddServiceOffering(cmd.ServiceOfferingName, price);

        var response = new CreateServiceOfferingResponse(
            serviceOffering.Id,
            serviceOffering.Name,
            serviceOffering.Price.Amount,
            serviceOffering.Price.Currency
        );

        return Result<CreateServiceOfferingResponse>.Created(response);
    }
}