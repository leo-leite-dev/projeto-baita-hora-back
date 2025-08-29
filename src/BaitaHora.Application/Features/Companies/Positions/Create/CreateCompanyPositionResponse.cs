namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed record CreateCompanyPositionResponse(
    Guid PositionId,
    Guid CompanyId,
    string PositionName,
    string AccessLevel
);