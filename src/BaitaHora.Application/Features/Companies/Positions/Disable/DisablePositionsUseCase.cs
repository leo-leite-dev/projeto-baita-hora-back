using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed class DisablePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly IPositionGuards _positionGuards;

    public DisablePositionsUseCase(
        ICompanyGuards companyGuards,
        IPositionGuards positionGuards)
    {
        _companyGuards = companyGuards;
        _positionGuards = positionGuards;
    }

    //PAREM AQUI, TALVEZ NAO PRECISE DE GetWithPositionsAndMembers EnsureNoActiveMembersAsync
    public async Task<Result<DisablePositionsResponse>> HandleAsync(
        DisablePositionsCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.GetWithPositionsAndMembers(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<DisablePositionsResponse>.FromError(compRes);

        var company = compRes.Value!;

        var ids = IdSet.Normalize(cmd.PositionIds);
        if (ids.Count == 0)
            return Result<DisablePositionsResponse>.BadRequest("Nenhum cargo válido informado.");

        var notFound = IdSet.MissingFrom(ids, company.Positions, p => p.Id);
        if (notFound.Count > 0)
            return Result<DisablePositionsResponse>.NotFound(
                $"Cargos não encontrados: {string.Join(", ", notFound)}");

        var blockedRes = await _positionGuards.EnsureNoActiveMembersAsync(cmd.CompanyId, ids, ct);

        foreach (var pos in company.Positions.Where(p => ids.Contains(p.Id)))
            pos.Deactivate();

        return Result<DisablePositionsResponse>.Ok(new(ids));
    }
}