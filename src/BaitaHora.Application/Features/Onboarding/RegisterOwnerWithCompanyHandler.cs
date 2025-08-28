using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Onboarding;

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