using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Domain.Features.Common.Exceptions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed class PatchServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyServiceOfferingGuards _serviceOfferingGuards;
    private readonly ICurrentCompany _currentCompany;

    public PatchServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyServiceOfferingGuards serviceOfferingGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _serviceOfferingGuards = serviceOfferingGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(
        PatchServiceOfferingCommand cmd,
        CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.ServiceOfferingName);
        var wantsPriceChange = (cmd.Amount.HasValue && cmd.Amount > 0)
                               || !string.IsNullOrWhiteSpace(cmd.Currency);

        if (!wantsRename && !wantsPriceChange)
            return Result<Unit>.BadRequest("Nenhum campo para atualizar.");

        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;

        var serviceRes = _serviceOfferingGuards.ValidateServiceOffering(company, cmd.ServiceOfferingId, requireActive: true);

        if (serviceRes.IsFailure)
            return Result<Unit>.FromError(serviceRes);

        var serviceOffering = serviceRes.Value!;

        if (wantsRename)
            company.RenameServiceOffering(cmd.ServiceOfferingId, cmd.ServiceOfferingName!);

        if (wantsPriceChange)
        {
            var current = serviceOffering.Price;

            if (!cmd.Amount.HasValue && !string.IsNullOrWhiteSpace(cmd.Currency))
                return Result<Unit>.BadRequest("Para trocar a moeda, informe também o amount.");

            var newAmount = cmd.Amount ?? current.Amount;
            var newCurrency = string.IsNullOrWhiteSpace(cmd.Currency)
                ? current.Currency
                : cmd.Currency!;

            if (!string.Equals(newCurrency, current.Currency, StringComparison.Ordinal))
                throw new CompanyException("Troca de moeda não é permitida neste endpoint.");

            var newPrice = Money.RequirePositive(newAmount, newCurrency);

            if (!newPrice.Equals(current))
                serviceOffering.ChangePrice(newPrice);
        }

        return Result<Unit>.NoContent();
    }
}