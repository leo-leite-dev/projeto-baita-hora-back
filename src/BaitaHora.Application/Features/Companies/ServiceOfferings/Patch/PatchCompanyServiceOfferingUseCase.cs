using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using BaitaHora.Domain.Common.ValueObjects;
using Microsoft.Extensions.Logging;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Catalog.Patch;

public sealed class PatchCompanyServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ILogger<PatchCompanyServiceOfferingUseCase> _log;

    public PatchCompanyServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ILogger<PatchCompanyServiceOfferingUseCase> log)
    {
        _companyGuards = companyGuards;
        _log = log;
    }

    public async Task<Result<PatchCompanyServiceOfferingResponse>> HandleAsync(
        PatchCompanyServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.ExistsCompany(cmd.CompanyId, ct);

        if (companyRes.IsFailure)
            return Result<PatchCompanyServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var serviceOffering = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);

        if (serviceOffering is null)
            return Result<PatchCompanyServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        if (!string.IsNullOrWhiteSpace(cmd.ServiceOfferingName))
            company.RenameServiceOffering(cmd.ServiceOfferingId, cmd.ServiceOfferingName!);

        if (cmd.Amount.HasValue || !string.IsNullOrWhiteSpace(cmd.Currency))
        {
            var newAmount = cmd.Amount ?? serviceOffering.Price.Amount;
            var newCurrency = string.IsNullOrWhiteSpace(cmd.Currency) ? serviceOffering.Price.Currency : cmd.Currency!;

            try
            {
                var newPrice = Money.Create(newAmount, newCurrency);

                // company.SetServiceOfferingPrice(cmd.ServiceOfferingId, newPrice);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Preço inválido ao patchar CompanyServiceOffering. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}", cmd.CompanyId, cmd.ServiceOfferingId);
                return Result<PatchCompanyServiceOfferingResponse>.BadRequest("Preço (amount/currency) inválido.");
            }
        }

        // if (cmd.PositionIds is not null)
        // {
        //     if (cmd.PositionIds.Count == 0)
        //         company.ClearServiceOfferingPositions(cmd.ServiceOfferingId);
        //     else
        //         company.SetServiceOfferingPositions(cmd.ServiceOfferingId, cmd.PositionIds);
        // }

        _log.LogInformation(
            "CompanyServiceOffering PATCH aplicado. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}",
            cmd.CompanyId, cmd.ServiceOfferingId);

        var updated = company.ServiceOfferings.First(s => s.Id == cmd.ServiceOfferingId);

        var response = new PatchCompanyServiceOfferingResponse(updated.Id, updated.ServiceOfferingName);

        return Result<PatchCompanyServiceOfferingResponse>.Ok(response);
    }
}