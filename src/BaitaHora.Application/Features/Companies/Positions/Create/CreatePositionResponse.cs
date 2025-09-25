namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed record CreatePositionResponse(
    Guid PositionId,
    Guid CompanyId,
    string PositionName,
    string AccessLevel
);