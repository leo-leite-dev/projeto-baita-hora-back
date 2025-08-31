namespace BaitaHora.Application.Features.Companies.Positions.Enable;

public sealed record ActivatePositionsResponse(
    IReadOnlyCollection<Guid> ServiceOfferingIds
);
