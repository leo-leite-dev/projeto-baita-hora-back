using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _positionGuards;
    private readonly ICurrentCompany _currentCompany;

    public PatchPositionUseCase(
        ICompanyGuards companyGuards,
        ICompanyPositionGuards positionGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _positionGuards = positionGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(PatchPositionCommand cmd, CancellationToken ct)
    {
        var wantsRename = !string.IsNullOrWhiteSpace(cmd.PositionName);
        var wantsAccessChange = cmd.AccessLevel.HasValue;
        var wantsServicesSet = cmd.SetServiceOfferingIds is not null;

        var companyRes = wantsAccessChange
            ? await _companyGuards.GetWithPositionsMembersAndServiceOfferings(_currentCompany.Id, ct)
            : await _companyGuards.GetWithPositionsAndServiceOfferings(_currentCompany.Id, ct);

        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;

        var positionRes = _positionGuards.ValidatePosition(company, cmd.PositionId, requireActive: true);

        if (positionRes.IsFailure)
            return Result<Unit>.FromError(positionRes);

        var position = positionRes.Value!;

        if (wantsRename)
            company.RenamePosition(position.Id, cmd.PositionName!);

        if (wantsAccessChange)
            company.ChangePositionAccessLevel(position.Id, cmd.AccessLevel!.Value, alignMembers: true);

        if (wantsServicesSet)
            company.AssignServicesToPosition(position.Id, cmd.SetServiceOfferingIds!);

        return Result<Unit>.NoContent();
    }
}