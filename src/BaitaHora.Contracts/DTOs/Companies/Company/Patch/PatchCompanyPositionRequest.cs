using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record PatchCompanyPositionRequest(
    string? PositionName,
    CompanyRole? AccessLevel 
);