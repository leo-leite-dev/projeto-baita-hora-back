using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public interface ICompanyPositionGuards
{
    Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId);
}

public sealed class CompanyPositionGuards : ICompanyPositionGuards
{
    public Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId)
    {
        var position = company.Positions.SingleOrDefault(p => p.Id == positionId);
        if (position is null)
            return Result<CompanyPosition>.BadRequest("Cargo inválido para esta empresa.");

        if (!position.IsActive)
            return Result<CompanyPosition>.BadRequest("Não é possível atribuir um cargo inativo.");

        if (position.CompanyId != company.Id)
            return Result<CompanyPosition>.BadRequest("Cargo não pertence a esta empresa.");

        return Result<CompanyPosition>.Ok(position);
    }
}