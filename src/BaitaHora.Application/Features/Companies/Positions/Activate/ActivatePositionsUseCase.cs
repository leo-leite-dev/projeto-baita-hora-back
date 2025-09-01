using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Activate;

public sealed class ActivatePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;

    public ActivatePositionsUseCase(
        ICompanyGuards companyGuards,
        ICompanyPositionGuards companyPositionGuards)
    {
        _companyGuards = companyGuards;
        _companyPositionGuards = companyPositionGuards;
    }

    public async Task<Result<ActivatePositionsResponse>> HandleAsync(
        ActivatePositionsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivatePositionsResponse>.FromError(companyRes);

        var posGuardRes = await _companyPositionGuards.ValidatePositionsForActivation(cmd.PositionIds, ct);
        if (posGuardRes.IsFailure)
            return Result<ActivatePositionsResponse>.FromError(posGuardRes);

        foreach (var position in posGuardRes.Value!)
            position.Activate();

        var activatedIds = posGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<ActivatePositionsResponse>.Ok(new(activatedIds));
    }
}