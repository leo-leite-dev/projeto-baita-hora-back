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

    public async Task<Result<PatchServiceOfferingResponse>> HandleAsync(
        PatchServiceOfferingCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.ServiceOfferingName);
        var wantsPriceChange = (cmd.Amount.HasValue && cmd.Amount > 0)
                               || !string.IsNullOrWhiteSpace(cmd.Currency);

        if (!wantsRename && !wantsPriceChange)
            throw new ArgumentException("Nenhum campo para atualizar.", nameof(cmd));

        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<PatchServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var serviceOffering = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (serviceOffering is null)
            throw new KeyNotFoundException("Serviço não encontrado.");

        var changed = false;

        if (wantsRename)
        {
            company.RenameServiceOffering(cmd.ServiceOfferingId, cmd.ServiceOfferingName!);
            changed = true;
        }

        if (wantsPriceChange)
        {
            var current = serviceOffering.Price;

            if (!cmd.Amount.HasValue && !string.IsNullOrWhiteSpace(cmd.Currency))
                throw new ArgumentException("Para trocar a moeda, informe também o amount.", nameof(cmd));

            var newAmount = cmd.Amount ?? current.Amount;
            var newCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? current.Currency : cmd.Currency!;

            if (!string.Equals(newCurrency, current.Currency, StringComparison.Ordinal))
                throw new CompanyException("Troca de moeda não é permitida neste endpoint.");

            var newPrice = Money.RequirePositive(newAmount, newCurrency);

            if (!newPrice.Equals(current))
            {
                serviceOffering.ChangePrice(newPrice);
                changed = true;
            }
        }

        var response = new PatchServiceOfferingResponse(serviceOffering.Id, serviceOffering.Name);
        return Result<PatchServiceOfferingResponse>.Ok(response);
    }
}