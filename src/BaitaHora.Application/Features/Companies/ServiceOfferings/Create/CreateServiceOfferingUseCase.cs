using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;
using DomainMoney = BaitaHora.Domain.Common.ValueObjects.Money;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public CreateServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(CreateServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;

        if (string.IsNullOrWhiteSpace(cmd.Currency))
            return Result<Unit>.FromError(Result.BadRequest("Moeda é obrigatória."));

        var price = DomainMoney.RequirePositive(cmd.Amount, cmd.Currency!);

        company.AddServiceOffering(cmd.ServiceOfferingName, price);

        return Result<Unit>.Created();
    }
}