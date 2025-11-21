using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Stats.Get.List;

public sealed class ListCompanyStatsHandler
    : IRequestHandler<ListCompanyStatsQuery, Result<CompanyStatsDto>>
{
    private readonly ListCompanyStatsUseCase _useCase;

    public ListCompanyStatsHandler(ListCompanyStatsUseCase useCase)
    {
        _useCase = useCase;
    }

    public Task<Result<CompanyStatsDto>> Handle(
        ListCompanyStatsQuery request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}