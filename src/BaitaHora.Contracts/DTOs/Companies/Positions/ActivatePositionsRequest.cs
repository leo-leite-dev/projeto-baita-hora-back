namespace BaitaHora.Contracts.DTOs.Companies.Positions;

public sealed record ActivatePositionsRequest(IEnumerable<Guid> PositionIds);
