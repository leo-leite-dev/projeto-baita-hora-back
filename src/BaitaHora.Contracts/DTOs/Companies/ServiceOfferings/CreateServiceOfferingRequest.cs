namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record CreateServiceOfferingRequest(
    string Name,
    decimal Amount,
    string Currency
);