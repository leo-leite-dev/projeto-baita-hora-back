namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record ActivatePositionsRequest(IEnumerable<Guid> PositionIds);
