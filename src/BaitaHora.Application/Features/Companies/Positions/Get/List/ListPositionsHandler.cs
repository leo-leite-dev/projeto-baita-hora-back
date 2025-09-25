using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ListPositions.Get.List;

public sealed class ListPositionsHandler
    : IRequestHandler<ListPositionsQuery, Result<IReadOnlyList<PositionDetails>>>
{
    private readonly ICompanyPositionRepository _positionRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListPositionsHandler(
        ICompanyPositionRepository positionRepository,
        ICurrentCompany currentCompany)
    {
        _positionRepository = positionRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<PositionDetails>>> Handle(
        ListPositionsQuery request, CancellationToken ct)
    {
        var list = await _positionRepository.ListAllPositionsByCompanyAsync(_currentCompany.Id, ct);

        return Result<IReadOnlyList<PositionDetails>>.Ok(list);
    }
}