using BaitaHora.Api.Contracts.Companies.Requests;
using BaitaHora.Application.Features.Companies.Commands;

namespace BaitaHora.Api.Contracts.Companies.Mappers;

public static class CompanyPositionsApiMapper
{
    public static RegisterCompanyPositionCommand ToCommand(
        this CreateCompanyPositionRequest r, Guid companyId)
        => new()
        {
            CompanyId = companyId,
            PositionName = r.PositionName,
            AccessLevel = r.AccessLevel
        };
}