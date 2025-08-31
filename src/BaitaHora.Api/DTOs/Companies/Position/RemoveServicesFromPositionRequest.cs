namespace BaitaHora.Contracts.DTOs.Companies.Company.Remove;

public sealed record RemoveServicesFromPositionRequest(
    IEnumerable<Guid> ServiceOfferingIds
);
