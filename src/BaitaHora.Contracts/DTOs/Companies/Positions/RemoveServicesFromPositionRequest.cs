namespace BaitaHora.Contracts.DTOs.Companies.Positions;

public sealed record RemoveServicesFromPositionRequest(
    IEnumerable<Guid> ServiceOfferingIds
);
