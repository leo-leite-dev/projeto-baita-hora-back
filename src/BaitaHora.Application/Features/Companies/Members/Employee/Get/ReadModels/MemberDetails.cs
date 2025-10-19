namespace BaitaHora.Application.Features.Companies.Members.Get.ReadModels;

public sealed record MemberDetails(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    string Role,
    string Position,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    DateTime JoinedAt
);