using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.UseCase;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Handlers;

public sealed class RegisterCompanyPositionHandler
    : IRequestHandler<RegisterCompanyPositionCommand, Result<CompanyPositionResponse>>
{
    private readonly RegisterCompanyPositionUseCase _useCase;

    public RegisterCompanyPositionHandler(RegisterCompanyPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<CompanyPositionResponse>> Handle(RegisterCompanyPositionCommand request, CancellationToken ct
    ) => _useCase.HandleAsync(request, ct);
}