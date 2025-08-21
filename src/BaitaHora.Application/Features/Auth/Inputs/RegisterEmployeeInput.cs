using BaitaHora.Application.Features.Users.DTOs;

namespace BaitaHora.Application.Features.Auth.Inputs;

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