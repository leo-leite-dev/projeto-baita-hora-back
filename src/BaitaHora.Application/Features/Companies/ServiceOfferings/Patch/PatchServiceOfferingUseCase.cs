using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Common.ValueObjects;
using Microsoft.Extensions.Logging;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed class PatchServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<PatchServiceOfferingUseCase> _log;

    public PatchServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        ILogger<PatchServiceOfferingUseCase> log)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _log = log;
    }

    public async Task<Result<PatchServiceOfferingResponse>> HandleAsync(
        PatchServiceOfferingCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.ServiceOfferingName);
        var wantsPriceChange = (cmd.Amount.HasValue && cmd.Amount > 0)
         || !string.IsNullOrWhiteSpace(cmd.Currency);

        if (!wantsRename)
            return Result<PatchServiceOfferingResponse>.BadRequest("Nenhum campo para atualizar.");

        var companyRes = await _companyGuards.GetWithServiceOfferingsOrNotFoundAsync(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<PatchServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var serviceOffering = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (serviceOffering is null)
            return Result<PatchServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        var changed = false;

        if (wantsRename)
        {
            try
            {
                company.RenameServiceOffering(cmd.ServiceOfferingId, cmd.ServiceOfferingName!);
                changed = true;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex,
                    "Falha ao renomear serviço. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}, Name={Name}",
                    cmd.CompanyId, cmd.ServiceOfferingId, cmd.ServiceOfferingName);
                return Result<PatchServiceOfferingResponse>.BadRequest(ex.Message);
            }
        }

        if (wantsPriceChange)
        {
            var current = serviceOffering.Price;

            if (!cmd.Amount.HasValue && !string.IsNullOrWhiteSpace(cmd.Currency))
            {
                return Result<PatchServiceOfferingResponse>.BadRequest("Para trocar a moeda, informe também o amount.");
            }

            var newAmount = cmd.Amount ?? current.Amount;
            var newCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? current.Currency : cmd.Currency!;

            try
            {
                if (!string.Equals(newCurrency, current.Currency, StringComparison.Ordinal))
                    return Result<PatchServiceOfferingResponse>.BadRequest("Troca de moeda não é permitida neste endpoint.");

                var newPrice = Money.RequirePositive(newAmount, newCurrency);

                if (!newPrice.Equals(current))
                {
                    serviceOffering.ChangePrice(newPrice);
                    changed = true;
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex,
                    "Preço inválido ao patchar ServiceOffering. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}, Amount={Amount}, Currency={Currency}",
                    cmd.CompanyId, cmd.ServiceOfferingId, cmd.Amount, cmd.Currency);
                return Result<PatchServiceOfferingResponse>.BadRequest("Preço (amount/currency) inválido.");
            }
        }

        if (!changed)
        {
            var unchanged = new PatchServiceOfferingResponse(serviceOffering.Id, serviceOffering.Name);
            return Result<PatchServiceOfferingResponse>.Ok(unchanged);
        }

        await _companyRepository.UpdateAsync(company, ct);

        _log.LogInformation(
            "ServiceOffering PATCH aplicado. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}",
            cmd.CompanyId, cmd.ServiceOfferingId);

        var response = new PatchServiceOfferingResponse(serviceOffering.Id, serviceOffering.Name);
        return Result<PatchServiceOfferingResponse>.Ok(response);
    }
}