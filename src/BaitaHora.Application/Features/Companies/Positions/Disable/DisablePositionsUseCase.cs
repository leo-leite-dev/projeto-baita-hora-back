using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;

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

    public async Task<Result<Unit>> HandleAsync(DisablePositionsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(_currentCompany.Id, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var posGuardRes = await _companyPositionGuards.ValidatePositionsForDesactivation(cmd.PositionIds, ct);
        if (posGuardRes.IsFailure)
            return Result<Unit>.FromError(posGuardRes);

        foreach (var pos in posGuardRes.Value!)
            pos.Desactivate();

        return Result<Unit>.NoContent();
    }
}