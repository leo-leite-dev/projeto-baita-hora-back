namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Enable;

public sealed record ActivateServiceOfferingsResponse(
    IReadOnlyCollection<Guid> ServiceOfferingIds
);
