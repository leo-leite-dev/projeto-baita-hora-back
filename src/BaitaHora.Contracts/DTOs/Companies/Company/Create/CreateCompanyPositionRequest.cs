using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record CreateCompanyPositionRequest(
    string PositionName,
    CompanyRole AccessLevel 
);
