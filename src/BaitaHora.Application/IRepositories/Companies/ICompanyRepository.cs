using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetWithServiceOfferingsAsync(Guid id, CancellationToken ct);
    Task<Company?> GetWithPositionAndServiceOfferingsAsync(Guid id, CancellationToken ct);
    Task<Company?> GetByIdWithPositionsAndMembersAsync(Guid companyId, CancellationToken ct);
    Task AddImageAsync(CompanyImage image, CancellationToken ct = default);
}
