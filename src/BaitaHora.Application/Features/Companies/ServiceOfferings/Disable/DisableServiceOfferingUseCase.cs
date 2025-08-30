using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable;

public sealed class DisableServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<DisableServiceOfferingUseCase> _log;

    public DisableServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        ILogger<DisableServiceOfferingUseCase> log)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _log = log;
    }

    public async Task<Result<DisableServiceOfferingResponse>> HandleAsync(
        DisableServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferingsOrNotFoundAsync(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<DisableServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;
        var serviceOffering = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (serviceOffering is null)
            return Result<DisableServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        try
        {
            serviceOffering.Deactivate();
        }
        catch (CompanyException ex)
        {
            _log.LogWarning(ex,
                "Erro ao desativar serviço. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}",
                cmd.CompanyId, cmd.ServiceOfferingId);

            return Result<DisableServiceOfferingResponse>.BadRequest(ex.Message);
        }

        await _companyRepository.UpdateAsync(company, ct);


        var response = new DisableServiceOfferingResponse(serviceOffering.Id);

        return Result<DisableServiceOfferingResponse>.Ok(response);
    }
}