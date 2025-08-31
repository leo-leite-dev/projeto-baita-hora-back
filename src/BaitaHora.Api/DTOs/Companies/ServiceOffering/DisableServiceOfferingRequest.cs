namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record DisableServiceOfferingsRequest(IEnumerable<Guid> ServiceOfferingIds);