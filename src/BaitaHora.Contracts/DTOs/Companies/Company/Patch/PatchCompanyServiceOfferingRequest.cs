namespace BaitaHora.Contracts.DTOs.Companies.Company.Patch;

public sealed record PatchCompanyServiceOfferingRequest(
    string? ServiceOfferingName,
    decimal? Amount,
    string? Currency,
    Guid? PositionId
);