using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Activate;

public sealed class ActivatePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;
    private readonly ICurrentCompany _currentCompany;

    public ActivatePositionsUseCase(
        ICompanyGuards companyGuards,
        ICompanyPositionGuards companyPositionGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _companyPositionGuards = companyPositionGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(ActivatePositionsCommand cmd, CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var companyRes = await _companyGuards.EnsureCompanyExists(companyId, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var posGuardRes = await _companyPositionGuards
            .ValidatePositionsForActivation(companyId, cmd.PositionIds, ct);

        if (posGuardRes.IsFailure)
            return Result<Unit>.FromError(posGuardRes);

        foreach (var position in posGuardRes.Value!)
            position.Activate();

        return Result<Unit>.NoContent();
    }
}