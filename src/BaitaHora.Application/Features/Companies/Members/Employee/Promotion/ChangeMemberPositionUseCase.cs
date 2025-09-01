using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Members.ChangePosition;

namespace BaitaHora.Application.Companies.Features.Members.Promotion;

public sealed class ChangeMemberPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public ChangeMemberPositionUseCase(ICompanyGuards companyGuards)
    {
        _companyGuards = companyGuards;
    }

    public async Task<Result<ChangeMemberPositionResponse>> HandleAsync(
        ChangeMemberPositionCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.GetWithPositionsAndMembers(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<ChangeMemberPositionResponse>.FromError(compRes);

        var company = compRes.Value!;

        var memberBefore = company.Members.Single(m => m.UserId == cmd.EmployeeId);
        var oldPosId = memberBefore.PrimaryPositionId;
        var oldRole = memberBefore.Role;

        var memberAfter = company.ChangeMemberPrimaryPosition(
            userId: cmd.EmployeeId,
            newPositionId: cmd.NewPositionId,
            alignRoleToPosition: cmd.AlignRoleToPosition 
        );

        var roleAligned = cmd.AlignRoleToPosition
                          && memberAfter.PrimaryPosition is not null
                          && memberAfter.Role == memberAfter.PrimaryPosition.AccessLevel;

        var response = new ChangeMemberPositionResponse(
            EmployeeId: cmd.EmployeeId,
            OldPositionId: oldPosId,
            NewPositionId: cmd.NewPositionId,
            RoleAligned: roleAligned
        );

        return Result<ChangeMemberPositionResponse>.Ok(response);
    }

}