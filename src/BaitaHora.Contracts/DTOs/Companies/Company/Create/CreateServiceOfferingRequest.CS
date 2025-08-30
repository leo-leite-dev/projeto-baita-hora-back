namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record CreateServiceOfferingRequest(
    string ServiceOfferingName,
    decimal Amount,
    string Currency
);