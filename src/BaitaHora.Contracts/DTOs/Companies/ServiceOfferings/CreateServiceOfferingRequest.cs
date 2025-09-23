namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record CreateServiceOfferingRequest(
    string ServiceOfferingName,
    decimal Amount,
    string Currency
);