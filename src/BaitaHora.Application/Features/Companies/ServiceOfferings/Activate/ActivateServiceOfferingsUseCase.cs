using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;

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

    public async Task<Result<Unit>> HandleAsync(ActivateServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var valRes = await _serviceOfferingGuards.ValidateServiceOfferingsForActivation(cmd.ServiceOfferingIds, ct);
        if (valRes.IsFailure)
            return Result<Unit>.FromError(valRes);

        foreach (var service in valRes.Value!)
            service.Activate();

        return Result<Unit>.NoContent();
    }
}