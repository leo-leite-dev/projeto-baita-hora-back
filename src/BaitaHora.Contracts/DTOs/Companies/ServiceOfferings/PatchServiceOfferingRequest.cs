namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record PatchServiceOfferingRequest(
    string? Name,
    decimal? Amount,
    string? Currency
);