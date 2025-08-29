using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using MediatR;

public sealed record CreateCompanyServiceOfferingCommand
    : IRequest<Result<CreateCompanyServiceOfferingResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public string ServiceOfferingName { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public IReadOnlyCollection<Guid>? PositionIds { get; init; }
}
