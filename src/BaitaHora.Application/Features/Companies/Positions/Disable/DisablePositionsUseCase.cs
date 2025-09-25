using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed class DisablePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;
    private readonly ICurrentCompany _currentCompany;

    public DisablePositionsUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany,
        ICompanyPositionGuards companyPositionGuards)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
        _companyPositionGuards = companyPositionGuards;
    }

    public async Task<Result<DisablePositionsResponse>> HandleAsync(
        DisablePositionsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);

        if (companyRes.IsFailure)
            return Result<DisablePositionsResponse>.FromError(companyRes);

        var posGuardRes = await _companyPositionGuards.ValidatePositionsForDeactivation(cmd.PositionIds, ct);
        if (posGuardRes.IsFailure)
            return Result<DisablePositionsResponse>.FromError(posGuardRes);

        foreach (var pos in posGuardRes.Value!)
            pos.Deactivate();

        var disabledIds = posGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<DisablePositionsResponse>.Ok(new(disabledIds));
    }
}