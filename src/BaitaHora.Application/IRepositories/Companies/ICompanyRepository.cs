using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetDetailsByIdAsync(Guid companyId, CancellationToken ct);

    Task AddImageAsync(CompanyImage image, CancellationToken ct = default);
}
