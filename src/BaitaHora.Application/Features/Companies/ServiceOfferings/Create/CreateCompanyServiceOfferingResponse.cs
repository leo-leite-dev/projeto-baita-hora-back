namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed record CreateCompanyServiceOfferingResponse(
    Guid ServiceId,
    string ServiceOfferingName,
    decimal Amount,
    string Currency
);