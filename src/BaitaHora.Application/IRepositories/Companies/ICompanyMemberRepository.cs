using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
    {
        Task<MemberAdminEditView?> GetByMemberIdAsync(Guid companyId, Guid memberId, CancellationToken ct);
        Task<MemberProfileDetails?> GetMemberFullDetailsAsync(Guid companyId, Guid memberId, CancellationToken ct);
        Task<CompanyMember?> GetMemberByIdAsync(Guid companyId, Guid memberId, CancellationToken ct); Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<CompanyMember>> GetByCompanyAndUserIdsAsync(Guid companyId, IReadOnlyCollection<Guid> userIds, CancellationToken ct = default);
        Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<IReadOnlyList<MemberDetails>> ListAllMembersByCompanyAsync(Guid companyId, CancellationToken ct);
    }
}