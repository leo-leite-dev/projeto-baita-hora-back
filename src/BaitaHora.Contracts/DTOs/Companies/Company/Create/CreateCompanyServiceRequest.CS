namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record CreateCompanyServiceOfferingRequest(
    string ServiceOfferingName,
    decimal Amount,
    string Currency
);