using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Feature.Customers;
using MediatR;

namespace BaitaHora.Application.Features.Customers.Create;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResponse>>
{
    private readonly CreateCustomerUseCase _useCase;

    public CreateCustomerHandler(CreateCustomerUseCase useCase)
        => _useCase = useCase;

    public Task<Result<CreateCustomerResponse>> Handle(
        CreateCustomerCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}