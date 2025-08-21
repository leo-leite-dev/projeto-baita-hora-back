using BaitaHora.Application.Auth.Commands;
using BaitaHora.Application.Auth.DTO.Responses;
using BaitaHora.Application.Common;
using MediatR;

namespace BaitaHora.Application.Auth.Handlers;

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