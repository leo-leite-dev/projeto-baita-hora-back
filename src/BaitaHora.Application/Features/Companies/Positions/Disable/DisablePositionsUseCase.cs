using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed class DisablePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;

    public DisablePositionsUseCase(
        ICompanyGuards companyGuards,
        ICompanyPositionGuards companyPositionGuards)
    {
        _companyGuards = companyGuards;
        _companyPositionGuards = companyPositionGuards;
    }

    public async Task<Result<DisablePositionsResponse>> HandleAsync(
        DisablePositionsCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<DisablePositionsResponse>.FromError(compRes);

        var posGuardRes = await _companyPositionGuards.ValidatePositionsForDeactivation(cmd.CompanyId, cmd.PositionIds, ct);
        if (posGuardRes.IsFailure)
            return Result<DisablePositionsResponse>.FromError(posGuardRes);

        foreach (var pos in posGuardRes.Value!)
            pos.Deactivate();

        var disabledIds = posGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<DisablePositionsResponse>.Ok(new(disabledIds));
    }
}