namespace BaitaHora.Contracts.DTOs.Companies.Positions;

public sealed record DisablePositionsRequest(IEnumerable<Guid> PositionIds);