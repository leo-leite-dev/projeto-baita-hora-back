
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyGuards
{
    Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct);
}