namespace BaitaHora.Contracts.DTOs.Companies.Company.Patch;

public sealed record PatchServiceOfferingRequest(
    string? ServiceOfferingName,
    decimal? Amount,
    string? Currency
);