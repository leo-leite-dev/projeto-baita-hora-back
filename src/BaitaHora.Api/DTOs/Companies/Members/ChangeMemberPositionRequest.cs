namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record ChangeMemberPositionRequest(
    Guid PositionId,
    bool AlignRoleToPosition = false
);