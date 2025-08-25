using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Contracts.DTOs.Companies;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyServicesApiMapper
{
    public static CreateCompanyServiceCommand ToCommand(this CreateCompanyServiceRequest r, Guid companyId)
        => new()
        {
            CompanyId = companyId,
            ServiceName = r.ServiceName,
            Amount = r.Amount,
            Currency = r.Currency,
            PositionIds = r.PositionIds ?? Array.Empty<Guid>()
        };
}
