namespace BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;

public sealed record RemoveServicesFromPositionResponse(
    Guid PositionId,
    IEnumerable<Guid> RemovedServiceOfferingIds
);