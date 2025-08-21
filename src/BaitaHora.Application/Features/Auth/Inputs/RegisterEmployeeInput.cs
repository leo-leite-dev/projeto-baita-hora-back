using BaitaHora.Application.Users.DTOs;

namespace BaitaHora.Application.Auth.Inputs;

public sealed record RegisterEmployeeInput(
    Guid CompanyId,
    Guid ActorUserId,
    string Email,
    string? Username,
    string RawPassword,
    int Role,
    Guid PositionId,
    UserProfileInput Profile
);