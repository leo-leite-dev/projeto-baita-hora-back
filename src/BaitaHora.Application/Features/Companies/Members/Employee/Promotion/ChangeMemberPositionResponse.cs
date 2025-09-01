namespace BaitaHora.Application.Companies.Features.Members.Promotion;

public sealed record ChangeMemberPositionResponse(
    Guid EmployeeId,
    Guid? OldPositionId,
    Guid NewPositionId,
    bool RoleAligned
);
