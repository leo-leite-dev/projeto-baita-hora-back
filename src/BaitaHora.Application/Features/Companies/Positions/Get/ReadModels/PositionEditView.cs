using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Companies.Features.Positions.Models;

public sealed record PositionEditView : PositionDetailsBase
{
    public CompanyRole AccessLevel { get; init; }
    public IReadOnlyList<ServiceDto> ServiceOfferings { get; init; } = Array.Empty<ServiceDto>();
}