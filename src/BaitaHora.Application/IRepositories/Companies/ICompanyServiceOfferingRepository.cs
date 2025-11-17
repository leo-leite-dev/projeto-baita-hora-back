using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyServiceOfferingRepository : IGenericRepository<CompanyServiceOffering>
    {
        Task<ServiceOfferingEditView?> GetByIdAsync(Guid companyId, Guid serviceOfferingId, CancellationToken ct);
        Task<IReadOnlyList<ServiceOfferingOptions>> ListServiceOfferingActiveForOptionsAsync(Guid companyId, string? search, int take, CancellationToken ct);
        Task<IReadOnlyList<ServiceOfferingDetails>> ListAllServicesByCompanyAsync(Guid companyId, CancellationToken ct);
        Task<IReadOnlyList<ServiceOfferingOptions>> ListServiceOfferingActiveForMemberOptionsAsync(Guid companyId, Guid memberId, string? search, int take, CancellationToken ct);
    }
}