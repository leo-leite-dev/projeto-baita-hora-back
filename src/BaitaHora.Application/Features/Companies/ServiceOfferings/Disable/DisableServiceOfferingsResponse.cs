namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable;

public sealed record DisableServiceOfferingsResponse(
    IReadOnlyCollection<Guid> ServiceOfferingIds
);