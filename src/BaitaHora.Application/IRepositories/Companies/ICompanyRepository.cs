using BaitaHora.Domain.Commons.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Entities.Companies;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<bool> IsCompanyNameTakenAsync(CompanyName companyName, Guid? excludingCompanyId, CancellationToken ct);
        Task<bool> IsCnpjTakenAsync(CNPJ cnpj, Guid? excludingCompanyId, CancellationToken ct);
        Task<bool> IsCompanyEmailTakenAsync(Email email, Guid? excludingCompanyId, CancellationToken ct);

        Task AddImageAsync(CompanyImage image, CancellationToken ct = default);
    }
}