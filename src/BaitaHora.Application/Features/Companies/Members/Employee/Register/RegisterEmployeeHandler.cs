using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Register;

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