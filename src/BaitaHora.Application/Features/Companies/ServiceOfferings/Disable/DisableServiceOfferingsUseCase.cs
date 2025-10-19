using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;

public sealed class DisableServiceOfferingsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyServiceOfferingGuards _serviceOfferingGuards;
    private readonly ICurrentCompany _currentCompany;

    public DisableServiceOfferingsUseCase(
        ICompanyGuards companyGuards,
        ICompanyServiceOfferingGuards serviceOfferingGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _serviceOfferingGuards = serviceOfferingGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result> HandleAsync(DisableServiceOfferingsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result.FromError(companyRes);

        var company = companyRes.Value!;

        var valRes = await _serviceOfferingGuards
            .ValidateServiceOfferingsForDesactivation(_currentCompany.Id, cmd.ServiceOfferingIds, ct);

        if (valRes.IsFailure)
            return Result.FromError(valRes);

        var services = valRes.Value!;
        var ids = services.Select(s => s.Id).ToArray();

        company.DetachServiceOfferingsFromAllPositions(ids);

        foreach (var s in services)
            s.Deactivate();

        return Result.NoContent();
    }
}