using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Feature.Schedules.Customers.Create;

public sealed class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly CreateCustomerUseCase _useCase;

    public CreateCustomerHandler(CreateCustomerUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
