namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed record DisablePositionsResponse(
    IReadOnlyCollection<Guid> PositionIds
);
