namespace BaitaHora.Application.Companies.Features.Members.Promotion;

public sealed record ChangeMemberPositionResponse(
    Guid MemberId,
    Guid? OldPositionId,
    Guid NewPositionId,
    string AccessLevel,
    bool RoleAligned
);
