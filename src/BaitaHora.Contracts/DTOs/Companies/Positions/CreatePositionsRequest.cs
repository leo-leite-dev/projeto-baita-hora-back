using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Positions;

public sealed record CreatePositionRequest(
    string PositionName,
    CompanyRole AccessLevel,
    IEnumerable<Guid> ServiceOfferingIds
);