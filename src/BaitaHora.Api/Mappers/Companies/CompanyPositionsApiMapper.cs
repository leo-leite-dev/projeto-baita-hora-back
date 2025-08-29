using BaitaHora.Application.Features.Companies.Positions.Create;
using BaitaHora.Application.Features.Companies.Positions.Patch;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyPositionsApiMappers
{
    public static CreateCompanyPositionCommand ToCommand(
        this CreateCompanyPositionRequest r, Guid companyId)
        => new CreateCompanyPositionCommand
        {
            CompanyId = companyId,
            PositionName = r.PositionName,
            AccessLevel = r.AccessLevel.ToDomain()
        };

    public static PatchCompanyPositionCommand ToCommand(
          this PatchCompanyPositionRequest r, Guid companyId, Guid positionId)
          => new PatchCompanyPositionCommand
          {
              CompanyId = companyId,
              PositionId = positionId,
              NewPositionName = r.PositionName,
              NewAccessLevel = r.AccessLevel?.ToDomain()
          };
}