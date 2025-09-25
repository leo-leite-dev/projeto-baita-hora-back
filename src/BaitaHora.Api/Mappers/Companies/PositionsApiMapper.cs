using BaitaHora.Application.Features.Companies.Positions.Activate;
using BaitaHora.Application.Features.Companies.Positions.Create;
using BaitaHora.Application.Features.Companies.Positions.Disable;
using BaitaHora.Application.Features.Companies.Positions.Patch;
using BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;
using BaitaHora.Contracts.DTOs.Companies.Positions;

namespace BaitaHora.Api.Mappers.Companies;

public static class PositionsApiMappers
{
    public static CreatePositionCommand ToCommand(
        this CreatePositionRequest r)
        => new CreatePositionCommand
        {
            PositionName = r.PositionName,
            AccessLevel = r.AccessLevel.ToDomain(),
            ServiceOfferingIds = r.ServiceOfferingIds
        };

    public static PatchPositionCommand ToCommand(
        this PatchPositionRequest r, Guid positionId)
        => new PatchPositionCommand
        {
            PositionId = positionId,
            NewPositionName = r.PositionName,
            NewAccessLevel = r.AccessLevel?.ToDomain(),
            SetServiceOfferingIds = (r.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static RemoveServicesFromPositionCommand ToCommand(
        this RemoveServicesFromPositionRequest r, Guid positionId)
        => new RemoveServicesFromPositionCommand
        {
            PositionId = positionId,
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>()).ToArray()
        };

    public static DisablePositionsCommand ToCommand(
        this DisablePositionsRequest r)
        => new DisablePositionsCommand
        {
            PositionIds = (r?.PositionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ActivatePositionsCommand ToCommand(
        this ActivatePositionsRequest r)
        => new()
        {
            PositionIds = (r?.PositionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };
}