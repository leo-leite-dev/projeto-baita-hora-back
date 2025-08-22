using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<Company?> GetByIdWithMembersAndPositionsAsync(Guid companyId, CancellationToken ct);

        Task AddImageAsync(CompanyImage image, CancellationToken ct = default);
    }
}