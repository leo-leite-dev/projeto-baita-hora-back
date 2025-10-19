using BaitaHora.Application.Features.Users.Dtos;

namespace BaitaHora.Application.Features.Companies.Members.Get.ReadModels;

public sealed record MemberProfileDetails(
    Guid Id,
    string Role,
    string? Position,
    bool IsActive,
    DateTime JoinedAt,
    UserDto User
);