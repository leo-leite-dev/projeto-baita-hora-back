using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record CreatePositionRequest(
    string PositionName,
    CompanyRole AccessLevel,
    IEnumerable<Guid> ServiceOfferingIds
);