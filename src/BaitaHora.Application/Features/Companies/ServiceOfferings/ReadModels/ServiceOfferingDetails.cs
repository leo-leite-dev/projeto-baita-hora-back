namespace BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;

public sealed record ServiceOfferingDetails(
    Guid Id,
    string Name,
    decimal Price,
    string Currency,
    bool IsActive,
    DateTimeOffset CreatedAtUtc
);