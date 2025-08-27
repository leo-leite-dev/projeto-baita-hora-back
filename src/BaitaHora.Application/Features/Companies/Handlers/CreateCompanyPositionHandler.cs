using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.UseCases;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Handlers;

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