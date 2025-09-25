namespace BaitaHora.Contracts.DTOS.Companies;

public sealed record CompanySummary(
    Guid Id,
    string Name,
    string? TradeName,
    bool IsActive,
    string? LogoUrl = null
);
