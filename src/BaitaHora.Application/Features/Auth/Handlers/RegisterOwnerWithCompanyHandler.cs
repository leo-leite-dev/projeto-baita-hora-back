using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Auth.UseCases;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Responses;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Handlers;

public sealed class RegisterOwnerWithCompanyHandler
    : IRequestHandler<RegisterOwnerWithCompanyCommand, Result<RegisterOwnerWithCompanyResponse>>
{
    private readonly RegisterOwnerWithCompanyUseCase _useCase;

    public RegisterOwnerWithCompanyHandler(RegisterOwnerWithCompanyUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RegisterOwnerWithCompanyResponse>> Handle(
        RegisterOwnerWithCompanyCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}