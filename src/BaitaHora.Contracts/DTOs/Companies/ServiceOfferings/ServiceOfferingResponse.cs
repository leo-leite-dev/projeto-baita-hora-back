namespace BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;

public sealed record ServiceOfferingResponse(
    Guid Id,
    string Name,
    decimal Price,
    string Currency,
    bool IsActive,
    DateTimeOffset CreatedAtUtc
);