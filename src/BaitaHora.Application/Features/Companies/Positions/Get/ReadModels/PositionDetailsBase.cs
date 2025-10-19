namespace BaitaHora.Application.Companies.Features.Positions.Models;

public abstract record PositionDetailsBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
}