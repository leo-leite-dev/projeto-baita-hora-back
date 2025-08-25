using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Contracts.DTOs.Companies;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyPositionsApiMapper
{
    public static CreateCompanyPositionCommand ToCommand(
        this CreateCompanyPositionRequest r, Guid companyId)
        => new()
        {
            CompanyId   = companyId,
            PositionName = r.PositionName,
            AccessLevel  = r.AccessLevel.ToDomain() 
        };
}