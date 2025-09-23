namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record DisableServiceOfferingsRequest(IEnumerable<Guid> ServiceOfferingIds);