using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Activate;

public sealed class ActivateServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<ActivateServiceOfferingUseCase> _log;

    public ActivateServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        ILogger<ActivateServiceOfferingUseCase> log)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _log = log;
    }

    public async Task<Result<ActivateServiceOfferingResponse>> HandleAsync(
        ActivateServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferingsOrNotFoundAsync(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivateServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;
        var service = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (service is null)
            return Result<ActivateServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        if (!service.IsActive)
        {
            try
            {
                service.Activate();
            }
            catch (CompanyException ex)
            {
                _log.LogWarning(ex,
                    "Regra de negócio violada ao ativar serviço. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}",
                    cmd.CompanyId, cmd.ServiceOfferingId);
                return Result<ActivateServiceOfferingResponse>.BadRequest(ex.Message);
            }

            await _companyRepository.UpdateAsync(company, ct);
        }

        return Result<ActivateServiceOfferingResponse>.Ok(new(cmd.ServiceOfferingId));
    }
}