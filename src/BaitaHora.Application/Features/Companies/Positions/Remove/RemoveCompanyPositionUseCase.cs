using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Common.Exceptions;

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
        var compRes = await _companyGuards.GetWithPositionsAndMembers(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<RemovePositionResponse>.FromError(compRes);

        var company = compRes.Value!;
        var position = company.Positions.FirstOrDefault(p => p.Id == cmd.PositionId);
        if (position is null)
            return Result<RemovePositionResponse>.NotFound("Cargo não encontrado.");

        try
        {
            company.RemovePosition(cmd.PositionId);
        }
        catch (CompanyException ex)
        {
            return Result<RemovePositionResponse>.BadRequest(ex.Message);
        }

        return Result<RemovePositionResponse>.Ok(new(cmd.PositionId));
    }
}