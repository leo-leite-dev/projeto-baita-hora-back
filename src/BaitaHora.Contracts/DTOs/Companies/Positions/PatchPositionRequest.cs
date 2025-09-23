using BaitaHora.Contracts.Enums;

namespace BaitaHora.Contracts.DTOs.Companies.Positions;

public sealed record PatchPositionRequest(
    string? PositionName,
    CompanyRole? AccessLevel,
    IEnumerable<Guid>? ServiceOfferingIds
);