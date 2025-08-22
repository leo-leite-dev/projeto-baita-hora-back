namespace BaitaHora.Application.Features.Companies.Responses;

public sealed record RegisterEmployeeResponse(
    Guid UserId,
    string FullName,
    string Username,
    string Email,
    Guid PositionId,
    string PositionName
);