using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Enable;

public sealed class ActivateServiceOfferingsUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public ActivateServiceOfferingsUseCase(ICompanyGuards companyGuards)
        => _companyGuards = companyGuards;

    public async Task<Result<ActivateServiceOfferingsResponse>> HandleAsync(
        ActivateServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithServiceOfferings(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivateServiceOfferingsResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var ids = IdSet.Normalize(cmd.ServiceOfferingIds);
        if (ids.Count == 0)
            return Result<ActivateServiceOfferingsResponse>.BadRequest("Nenhum serviço válido informado.");

        var notFound = IdSet.MissingFrom(ids, company.ServiceOfferings, s => s.Id);
        if (notFound.Count > 0)
            return Result<ActivateServiceOfferingsResponse>.NotFound($"Serviços não encontrados: {string.Join(", ", notFound)}");

        foreach (var s in company.ServiceOfferings.Where(x => ids.Contains(x.Id)))
            s.Activate();

        return Result<ActivateServiceOfferingsResponse>.Ok(new(ids));
    }
}