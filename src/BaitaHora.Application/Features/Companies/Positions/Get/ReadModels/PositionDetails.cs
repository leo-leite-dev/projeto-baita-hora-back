using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;

public sealed record PositionDetails(
    Guid Id,
    string Name,
    CompanyRole AccessLevel,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdateAtUtc,
    IReadOnlyList<ServiceDto> ServiceOfferings);

public sealed record ServiceDto(Guid Id, string Name);
