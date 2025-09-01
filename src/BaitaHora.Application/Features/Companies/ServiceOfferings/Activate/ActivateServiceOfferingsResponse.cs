namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Activate;

public sealed record ActivateServiceOfferingsResponse(
    IReadOnlyCollection<Guid> ServiceOfferingIds
);
