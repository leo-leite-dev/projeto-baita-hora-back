using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Companies.Stats.Get.List;

public sealed class ListCompanyStatsUseCase
{
    private readonly ICurrentCompany _currentCompany;
    private readonly ICompanyStatsReadRepository _statsRepository;

    public ListCompanyStatsUseCase(
        ICurrentCompany currentCompany,
        ICompanyStatsReadRepository statsRepository)
    {
        _currentCompany = currentCompany;
        _statsRepository = statsRepository;
    }

    public async Task<Result<CompanyStatsDto>> HandleAsync(
        ListCompanyStatsQuery query, CancellationToken ct)
    {
        if (!_currentCompany.HasValue)
            return Result<CompanyStatsDto>.Forbidden("Empresa não selecionada.");

        var dateUtc = query.DateUtc ?? DateTime.UtcNow;

        var stats = await _statsRepository
            .GetStatsAsync(_currentCompany.Id, dateUtc, ct);

        return Result<CompanyStatsDto>.Ok(stats);
    }
}
