using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyServiceOfferingRepository : IGenericRepository<CompanyServiceOffering>
    {
        Task<ServiceOfferingDetails?> GetByIdAsync(Guid companyId, Guid serviceOfferingId, CancellationToken ct);
        Task<IReadOnlyList<ServiceOfferingComboItem>> ListActiveForComboAsync(Guid companyId, string? search, int take, CancellationToken ct);
        Task<IReadOnlyList<ServiceOfferingDetails>> ListAllServicesByCompanyAsync(Guid companyId, CancellationToken ct);
    }
}