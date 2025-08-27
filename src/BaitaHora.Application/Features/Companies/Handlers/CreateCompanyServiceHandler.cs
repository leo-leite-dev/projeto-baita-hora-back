using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.UseCases;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Handlers;

public sealed class CreateCompanyServiceHandler
    : IRequestHandler<CreateCompanyServiceCommand, Result<CreateCompanyServiceResponse>>
{
    private readonly CreateCompanyServiceUseCase _useCase;
    public CreateCompanyServiceHandler(CreateCompanyServiceUseCase useCase)
    => _useCase = useCase;

    public Task<Result<CreateCompanyServiceResponse>> Handle(
        CreateCompanyServiceCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}