using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Positions.Get.Combo;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Get.Options;

public sealed class ListActivePositionsOptionsHandler
    : IRequestHandler<ListActivePositionsOptionsQuery, Result<IReadOnlyList<PositionOptions>>>
{
    private readonly ICompanyPositionRepository _positionRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListActivePositionsOptionsHandler(
        ICompanyPositionRepository positionRepository,
        ICurrentCompany currentCompany)
    {
        _positionRepository = positionRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<PositionOptions>>> Handle(
        ListActivePositionsOptionsQuery request, CancellationToken ct)
    {
        var items = await _positionRepository.ListActivePositionForOptionsAsync(
            _currentCompany.Id, request.Search, request.Take, ct);

        return Result<IReadOnlyList<PositionOptions>>.Ok(items);
    }
}