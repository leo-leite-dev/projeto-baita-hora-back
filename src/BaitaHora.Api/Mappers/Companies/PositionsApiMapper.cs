using BaitaHora.Application.Features.Companies.Positions.Activate;
using BaitaHora.Application.Features.Companies.Positions.Create;
using BaitaHora.Application.Features.Companies.Positions.Disable;
using BaitaHora.Application.Features.Companies.Positions.Patch;
using BaitaHora.Application.Features.Companies.Positions.Remove;
using BaitaHora.Contracts.DTOs.Companies.Positions;

namespace BaitaHora.Api.Mappers.Companies;

public static class PositionsApiMappers
{
    public static CreatePositionCommand ToCommand(
        this CreatePositionRequest r, Guid companyId)
        => new CreatePositionCommand
        {
            CompanyId = companyId,
            PositionName = r.Name,
            AccessLevel = r.AccessLevel.ToDomain(),
            ServiceOfferingIds = r.ServiceOfferingIds
        };

    public static PatchPositionCommand ToCommand(
        this PatchPositionRequest r, Guid positionId, Guid companyId)
        => new PatchPositionCommand
        {
            PositionId = positionId,
            CompanyId = companyId,
            PositionName = r.Name,
            AccessLevel = r.AccessLevel?.ToDomain(),
            SetServiceOfferingIds = r.ServiceOfferingIds is null
                ? null
                : r.ServiceOfferingIds
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToArray()
        };

    public static RemovePositionCommand ToCommand(
        this Guid positionId, Guid companyId)
        => new RemovePositionCommand
        {
            PositionId = positionId,
            CompanyId = companyId
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
        => new ActivatePositionsCommand
        {
            CompanyId = companyId,
            PositionIds = (r?.PositionIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };
}