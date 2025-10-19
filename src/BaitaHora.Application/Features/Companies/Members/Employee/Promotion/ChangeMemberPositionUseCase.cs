using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Members.ChangePosition;

namespace BaitaHora.Application.Companies.Features.Members.Promotion;

public sealed class ChangeMemberPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICurrentCompany _currentCompany;

    public ChangeMemberPositionUseCase(
        ICompanyGuards companyGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<ChangeMemberPositionResponse>> HandleAsync(ChangeMemberPositionCommand cmd, CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var compRes = await _companyGuards.GetWithPositionsAndMembers(companyId, ct);
        if (compRes.IsFailure)
            return Result<ChangeMemberPositionResponse>.FromError(compRes);

        var company = compRes.Value!;
        var memberBefore = company.Members.Single(m => m.Id == cmd.MemberId);

        var oldPosId = memberBefore.PrimaryPositionId;

        var memberAfter = company.ChangeMemberPrimaryPosition(
            userId: memberBefore.UserId,
            newPositionId: cmd.NewPositionId,
            alignRoleToPosition: cmd.AlignRoleToPosition
        );

        var roleAligned = cmd.AlignRoleToPosition
                          && memberAfter.PrimaryPosition is not null
                          && memberAfter.Role == memberAfter.PrimaryPosition.AccessLevel;

        var response = new ChangeMemberPositionResponse(
            MemberId: cmd.MemberId,
            OldPositionId: oldPosId,
            NewPositionId: cmd.NewPositionId,
            AccessLevel: memberAfter.Role.ToString(),
            RoleAligned: roleAligned
        );

        return Result<ChangeMemberPositionResponse>.Ok(response);
    }
}