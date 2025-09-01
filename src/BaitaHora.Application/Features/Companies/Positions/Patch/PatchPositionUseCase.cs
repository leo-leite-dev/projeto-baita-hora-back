using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public PatchPositionUseCase(ICompanyGuards companyGuards)
        => _companyGuards = companyGuards;

    public async Task<Result<PatchPositionResponse>> HandleAsync(
        PatchPositionCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.NewPositionName);
        var wantsAccessChange = cmd.NewAccessLevel.HasValue;
        var wantsServicesSet = cmd.SetServiceOfferingIds is not null;

        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<PatchPositionResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var position = company.Positions.SingleOrDefault(p => p.Id == cmd.PositionId && p.IsActive);
        if (position is null)
            return Result<PatchPositionResponse>.NotFound("Cargo não encontrado ou inativo.");

        var changed = false;

        if (wantsRename)
        {
            company.RenamePosition(cmd.PositionId, cmd.NewPositionName!);
            changed = true;
        }

        if (wantsAccessChange)
        {
            company.ChangePositionAccessLevel(cmd.PositionId, cmd.NewAccessLevel!.Value, alignMembers: true);
            changed = true;
        }

        if (wantsServicesSet)
        {
            company.AssignServicesToPosition(cmd.PositionId, cmd.SetServiceOfferingIds!);
            changed = true;
        }

        if (!changed)
            return Result<PatchPositionResponse>.Ok(new(position.Id, position.Name));

        return Result<PatchPositionResponse>.Ok(new(position.Id, position.Name));
    }
}