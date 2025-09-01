namespace BaitaHora.Application.Features.Companies.Positions.Activate;

public sealed record ActivatePositionsResponse(
    IReadOnlyCollection<Guid> ServiceOfferingIds
);
