using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;

public sealed class ActivateServiceOfferingsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyServiceOfferingGuards _serviceOfferingGuards;

    public ActivateServiceOfferingsUseCase(
        ICompanyGuards companyGuards,
        ICompanyServiceOfferingGuards serviceOfferingGuards)
    {
        _companyGuards = companyGuards;
        _serviceOfferingGuards = serviceOfferingGuards;
    }

    public async Task<Result<ActivateServiceOfferingsResponse>> HandleAsync(
        ActivateServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivateServiceOfferingsResponse>.FromError(companyRes);

        var posGuardRes = await _serviceOfferingGuards.ValidateServiceOfferingsForActivation(cmd.ServiceOfferingIds, ct);
        if (posGuardRes.IsFailure)
            return Result<ActivateServiceOfferingsResponse>.FromError(posGuardRes);

        foreach (var position in posGuardRes.Value!)
            position.Activate();

        var activatedIds = posGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<ActivateServiceOfferingsResponse>.Ok(new(activatedIds));
    }
}