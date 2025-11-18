using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Companies.Features.Positions.Models;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Get.ById;

public sealed class GetPositionByIdHandler
    : IRequestHandler<GetPositionByIdQuery, Result<PositionEditView>>
{
    private readonly ICompanyPositionRepository _positionRepository;
    private readonly ICurrentCompany _currentCompany;

    public GetPositionByIdHandler(
        ICompanyPositionRepository positionRepository,
        ICurrentCompany currentCompany)
    {
        _positionRepository = positionRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<PositionEditView>> Handle(GetPositionByIdQuery request, CancellationToken ct)
    {
        var dto = await _positionRepository.GetByPositionIdAsync(_currentCompany.Id, request.PositionId, ct);

        return dto is null
            ? Result<PositionEditView>.NotFound("Cargo não encontrado.")
            : Result<PositionEditView>.Ok(dto);
    }
}