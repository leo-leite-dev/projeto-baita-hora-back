namespace BaitaHora.Application.Companies.Features.Positions.Models;

public sealed record PositionDetails : PositionDetailsBase
{
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? UpdatedAtUtc { get; init; }
    public IReadOnlyList<ServiceDto> ServiceOfferings { get; init; } = Array.Empty<ServiceDto>();
}

public sealed record ServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
}