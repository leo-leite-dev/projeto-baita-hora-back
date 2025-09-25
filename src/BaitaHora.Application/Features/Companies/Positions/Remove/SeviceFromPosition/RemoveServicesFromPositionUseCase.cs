using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;

public sealed class RemoveServicesFromPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public RemoveServicesFromPositionUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<RemoveServicesFromPositionResponse>> HandleAsync(
        RemoveServicesFromPositionCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<RemoveServicesFromPositionResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        company.RemoveServicesFromPosition(cmd.PositionId, cmd.ServiceOfferingIds);

        return Result<RemoveServicesFromPositionResponse>.Ok(
            new(cmd.PositionId, cmd.ServiceOfferingIds));
    }
}