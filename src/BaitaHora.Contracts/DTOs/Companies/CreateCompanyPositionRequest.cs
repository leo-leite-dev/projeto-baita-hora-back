using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies;

public sealed record CreateCompanyPositionRequest(
    string PositionName,
    CompanyRole AccessLevel 
);
