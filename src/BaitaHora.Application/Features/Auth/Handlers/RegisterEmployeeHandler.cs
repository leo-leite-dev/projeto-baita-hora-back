using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Commands;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Handlers;

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