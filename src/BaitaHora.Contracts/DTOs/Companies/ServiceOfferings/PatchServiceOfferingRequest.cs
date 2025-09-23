namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record PatchServiceOfferingRequest(
    string? ServiceOfferingName,
    decimal? Amount,
    string? Currency
);