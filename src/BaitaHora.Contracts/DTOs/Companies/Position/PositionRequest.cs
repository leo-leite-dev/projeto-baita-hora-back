using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Position;

public sealed record CreatePositionRequest(
    string PositionName,
    CompanyRole AccessLevel,
    IEnumerable<Guid>? ServiceOfferingIds
);