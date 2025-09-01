using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed class RemovePositionUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public RemovePositionUseCase(
        ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<RemovePositionResponse>> HandleAsync(
        RemovePositionCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndMembers(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<RemovePositionResponse>.FromError(companyRes);

        var company = companyRes.Value!;
        var position = company.Positions.FirstOrDefault(p => p.Id == cmd.PositionId);
        if (position is null)
            return Result<RemovePositionResponse>.NotFound("Cargo não encontrado.");

        company.RemovePosition(cmd.PositionId);

        return Result<RemovePositionResponse>.Ok(new(cmd.PositionId));
    }
}