using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed class PatchServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public PatchServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result> HandleAsync(
        PatchServiceOfferingCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.ServiceOfferingName);
        var wantsPriceChange = (cmd.Amount.HasValue && cmd.Amount > 0)
                               || !string.IsNullOrWhiteSpace(cmd.Currency);

        if (!wantsRename && !wantsPriceChange)
            return Result.BadRequest("Nenhum campo para atualizar.");

        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result.FromError(companyRes);

        var company = companyRes.Value!;

        var serviceOffering = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (serviceOffering is null)
            return Result.NotFound("Serviço não encontrado.");

        if (wantsRename)
            company.RenameServiceOffering(cmd.ServiceOfferingId, cmd.ServiceOfferingName!);

        if (wantsPriceChange)
        {
            var current = serviceOffering.Price;

            if (!cmd.Amount.HasValue && !string.IsNullOrWhiteSpace(cmd.Currency))
                return Result.BadRequest("Para trocar a moeda, informe também o amount.");

            var newAmount = cmd.Amount ?? current.Amount;
            var newCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? current.Currency : cmd.Currency!;

            if (!string.Equals(newCurrency, current.Currency, StringComparison.Ordinal))
                throw new CompanyException("Troca de moeda não é permitida neste endpoint.");

            var newPrice = Money.RequirePositive(newAmount, newCurrency);

            if (!newPrice.Equals(current))
                serviceOffering.ChangePrice(newPrice);
        }

        return Result.NoContent();
    }
}