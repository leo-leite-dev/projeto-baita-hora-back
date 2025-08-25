using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.UseCase;
using MediatR;

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