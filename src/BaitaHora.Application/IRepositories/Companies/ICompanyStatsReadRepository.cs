namespace BaitaHora.Application.IRepositories.Companies;

using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;

public interface ICompanyStatsReadRepository
{
    Task<CompanyStatsDto> GetStatsAsync(Guid companyId, DateTime dateUtc, CancellationToken ct = default);
}