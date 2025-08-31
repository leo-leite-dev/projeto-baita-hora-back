using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable;

public sealed class DisableServiceOfferingsUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public DisableServiceOfferingsUseCase(ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<DisableServiceOfferingsResponse>> HandleAsync(
        DisableServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferings(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<DisableServiceOfferingsResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var ids = IdSet.Normalize(cmd.ServiceOfferingIds);
        if (ids.Count == 0)
            return Result<DisableServiceOfferingsResponse>.BadRequest("Nenhum serviço válido informado.");

        var notFound = IdSet.MissingFrom(ids, company.ServiceOfferings, s => s.Id);
        if (notFound.Count > 0)
            return Result<DisableServiceOfferingsResponse>.NotFound($"Serviços não encontrados: {string.Join(", ", notFound)}");

        foreach (var s in company.ServiceOfferings.Where(x => ids.Contains(x.Id)))
            s.Deactivate();

        return Result<DisableServiceOfferingsResponse>.Ok(new(ids));
    }
}