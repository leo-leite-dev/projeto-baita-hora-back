using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;

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

    public async Task<Result<Unit>> HandleAsync(
        DisableServiceOfferingsCommand cmd,
        CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(companyId, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;

        var valRes = await _serviceOfferingGuards
            .ValidateServiceOfferingsForDesactivation(companyId, cmd.ServiceOfferingIds, ct);

        if (valRes.IsFailure)
            return Result<Unit>.FromError(valRes);

        var services = valRes.Value!;
        var ids = services.Select(s => s.Id).ToArray();

        company.DetachServiceOfferingsFromAllPositions(ids);

        foreach (var service in services)
            service.Desactivate();

        return Result<Unit>.NoContent();
    }
}