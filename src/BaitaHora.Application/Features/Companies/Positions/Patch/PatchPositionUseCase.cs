using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public PatchPositionUseCase(ICompanyGuards companyGuards, ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result> HandleAsync(PatchPositionCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.PositionName);
        var wantsAccessChange = cmd.AccessLevel.HasValue;
        var wantsServicesSet = cmd.SetServiceOfferingIds is not null;

        var companyRes = wantsAccessChange
            ? await _companyGuards.GetWithPositionsMembersAndServiceOfferings(_currentCompany.Id, ct)
            : await _companyGuards.GetWithPositionsAndServiceOfferings(_currentCompany.Id, ct);

        if (companyRes.IsFailure)
            return Result.FromError(companyRes);

        var company = companyRes.Value!;
        var position = company.Positions.SingleOrDefault(p => p.Id == cmd.PositionId && p.IsActive);
        if (position is null)
            return Result.NotFound("Cargo não encontrado ou inativo.");

        if (wantsRename)
            company.RenamePosition(cmd.PositionId, cmd.PositionName!);

        if (wantsAccessChange)
            company.ChangePositionAccessLevel(cmd.PositionId, cmd.AccessLevel!.Value, alignMembers: true);

        if (wantsServicesSet)
            company.AssignServicesToPosition(cmd.PositionId, cmd.SetServiceOfferingIds!);

        return Result.NoContent();
    }
}