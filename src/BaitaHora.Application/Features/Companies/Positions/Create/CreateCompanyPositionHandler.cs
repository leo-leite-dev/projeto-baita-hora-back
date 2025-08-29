using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreateCompanyPositionHandler
    : IRequestHandler<CreateCompanyPositionCommand, Result<CreateCompanyPositionResponse>>
{
    private readonly CreateCompanyPositionUseCase _useCase;

    public CreateCompanyPositionHandler(CreateCompanyPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<CreateCompanyPositionResponse>>  Handle(
        CreateCompanyPositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}