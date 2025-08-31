namespace BaitaHora.Contracts.DTOs.Companies.Positions.Disable;

public sealed record DisablePositionsRequest(IEnumerable<Guid> PositionIds);