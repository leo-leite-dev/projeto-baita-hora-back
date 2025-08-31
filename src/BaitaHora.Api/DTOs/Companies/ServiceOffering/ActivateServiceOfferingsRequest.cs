namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record ActivateServiceOfferingsRequest(IEnumerable<Guid> ServiceOfferingIds);
