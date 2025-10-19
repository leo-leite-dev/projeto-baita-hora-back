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

    public async Task<Result> HandleAsync(ActivateServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result.FromError(companyRes);

        var valRes = await _serviceOfferingGuards.ValidateServiceOfferingsForActivation(cmd.ServiceOfferingIds, ct);
        if (valRes.IsFailure)
            return Result.FromError(valRes);

        foreach (var service in valRes.Value!)
            service.Activate();

        return Result.NoContent();
    }
}