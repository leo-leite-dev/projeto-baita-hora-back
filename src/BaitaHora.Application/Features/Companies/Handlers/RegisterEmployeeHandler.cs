using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.UseCase;
using BaitaHora.Application.Features.Companies.Responses;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Handlers;

public sealed class RegisterEmployeeHandler
    : IRequestHandler<RegisterEmployeeCommand, Result<RegisterEmployeeResponse>>
{
    private readonly RegisterEmployeeUseCase _useCase;

    public RegisterEmployeeHandler(RegisterEmployeeUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RegisterEmployeeResponse>> Handle(
        RegisterEmployeeCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}