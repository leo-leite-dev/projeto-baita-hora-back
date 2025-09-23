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
        this CreatePositionRequest r, Guid companyId)
        => new CreatePositionCommand
        {
            CompanyId = companyId,
            PositionName = r.PositionName,
            AccessLevel = r.AccessLevel.ToDomain(),
            ServiceOfferingIds = r.ServiceOfferingIds
        };

    public static PatchPositionCommand ToCommand(
        this PatchPositionRequest r, Guid companyId, Guid positionId)
        => new PatchPositionCommand
        {
            CompanyId = companyId,
            PositionId = positionId,
            NewPositionName = r.PositionName,
            NewAccessLevel = r.AccessLevel?.ToDomain(),
            SetServiceOfferingIds = (r.ServiceOfferingIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static RemoveServicesFromPositionCommand ToCommand(
        this RemoveServicesFromPositionRequest r, Guid companyId, Guid positionId)
        => new RemoveServicesFromPositionCommand
        {
            CompanyId = companyId,
            PositionId = positionId,
            ServiceOfferingIds = (r?.ServiceOfferingIds ?? Enumerable.Empty<Guid>()).ToArray()
        };

    public static DisablePositionsCommand ToCommand(
        this DisablePositionsRequest r, Guid companyId)
        => new DisablePositionsCommand
        {
            CompanyId = companyId,
            PositionIds = (r?.PositionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ActivatePositionsCommand ToCommand(
        this ActivatePositionsRequest r, Guid companyId)
        => new()
        {
            CompanyId = companyId,
            PositionIds = (r?.PositionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };
}