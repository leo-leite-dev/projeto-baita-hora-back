using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
    {
        Task<MemberAdminEditView?> GetByMemberIdAsync(Guid companyId, Guid memberId, CancellationToken ct);
        Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<MemberProfileDetails?> GetMemberFullDetailsAsync(Guid companyId, Guid memberId, CancellationToken ct);
        Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<IReadOnlyList<MemberDetails>> ListAllMembersByCompanyAsync(Guid companyId, CancellationToken ct);
        Task<CompanyMember?> GetByIdWithPositionAsync(Guid companyId, Guid memberId, CancellationToken ct = default);
    }
}