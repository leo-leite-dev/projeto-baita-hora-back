namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed record CreateServiceOfferingResponse(
    Guid ServiceId,
    string ServiceOfferingName,
    decimal Amount,
    string Currency
);