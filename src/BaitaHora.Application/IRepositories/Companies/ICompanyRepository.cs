using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<Company?> GetWithServiceOfferingsAsync(Guid id, CancellationToken ct);
    Task<Company?> GetWithPositionAndServiceOfferingsAsync(Guid id, CancellationToken ct);
    Task<Company?> GetByIdWithPositionsAndMembersAsync(Guid companyId, CancellationToken ct);
    Task<Company?> GetWithPositionsMembersAndServiceOfferingsAsync(Guid companyId, CancellationToken ct);
    Task AddImageAsync(CompanyImage image, CancellationToken ct = default);

    Task<List<CompanyMember>> GetMembersByUserIdsAsync(Guid companyId, IReadOnlyCollection<Guid> userIds, CancellationToken ct = default);
    Task<List<CompanyPosition>> GetPositionsByIdsAsync(Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct = default);
    Task<List<CompanyServiceOffering>> GetServiceOfferingsByIdsAsync(Guid companyId, IReadOnlyCollection<Guid> ids, CancellationToken ct = default);
}
