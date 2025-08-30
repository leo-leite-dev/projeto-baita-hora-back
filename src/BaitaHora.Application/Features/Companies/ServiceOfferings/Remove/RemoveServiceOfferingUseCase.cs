using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed class RemoveServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<RemoveServiceOfferingUseCase> _log;

    public RemoveServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        ILogger<RemoveServiceOfferingUseCase> log)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _log = log;
    }

    public async Task<Result<RemoveServiceOfferingResponse>> HandleAsync(
        RemoveServiceOfferingCommand cmd, CancellationToken ct)
    {
        // 2) Carrega agregado mínimo (ServiceOfferings)
        var companyRes = await _companyGuards.GetWithServiceOfferingsOrNotFoundAsync(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<RemoveServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;
        var service = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (service is null)
            return Result<RemoveServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        // 3) Regras de domínio adicionais antes de remover (se houver)
        // Ex.: se houver agendamentos ativos, impedir remoção e sugerir disable:
        // if (company.HasActiveAppointmentsForService(service.Id))
        //     return Result<RemoveServiceOfferingResponse>.Conflict("Há agendamentos ativos para este serviço. Desative-o em vez de remover.");

        // 4) Remoção
        try
        {
            company.RemoveServiceOffering(service.Id);
        }
        catch (CompanyException ex)
        {
            _log.LogWarning(ex,
                "Regra de negócio violada ao remover serviço. CompanyId={CompanyId}, ServiceOfferingId={ServiceOfferingId}",
                cmd.CompanyId, cmd.ServiceOfferingId);

            return Result<RemoveServiceOfferingResponse>.BadRequest(ex.Message);
        }

        await _companyRepository.UpdateAsync(company, ct);

        return Result<RemoveServiceOfferingResponse>.Ok(new(cmd.ServiceOfferingId));
    }
}