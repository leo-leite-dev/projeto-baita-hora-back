namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record ActivateServiceOfferingsRequest(IEnumerable<Guid> ServiceOfferingIds);
