namespace BaitaHora.Application.Abstractions.Companies;

public sealed record CompanySummary(
    Guid Id,
    string Name,
    string? TradeName,
    bool IsActive,
    string? LogoUrl = null
);