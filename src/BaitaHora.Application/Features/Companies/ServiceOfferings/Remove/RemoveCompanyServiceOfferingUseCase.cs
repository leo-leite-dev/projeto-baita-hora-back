using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed class RemoveServiceOfferingUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public RemoveServiceOfferingUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<RemoveServiceOfferingResponse>> HandleAsync(
        RemoveServiceOfferingCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<RemoveServiceOfferingResponse>.FromError(companyRes);

        var company = companyRes.Value!;
        var service = company.ServiceOfferings.FirstOrDefault(s => s.Id == cmd.ServiceOfferingId);
        if (service is null)
            return Result<RemoveServiceOfferingResponse>.NotFound("Serviço não encontrado.");

        company.RemoveServiceOffering(service.Id);

        return Result<RemoveServiceOfferingResponse>.Ok(new(cmd.ServiceOfferingId));
    }
}