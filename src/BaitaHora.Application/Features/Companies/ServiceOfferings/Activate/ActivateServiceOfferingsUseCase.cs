using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;

public sealed class ActivateServiceOfferingsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyServiceOfferingGuards _serviceOfferingGuards;
    private readonly ICurrentCompany _currentCompany;

    public ActivateServiceOfferingsUseCase(
        ICompanyGuards companyGuards,
        ICompanyServiceOfferingGuards serviceOfferingGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _serviceOfferingGuards = serviceOfferingGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<ActivateServiceOfferingsResponse>> HandleAsync(
        ActivateServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);
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