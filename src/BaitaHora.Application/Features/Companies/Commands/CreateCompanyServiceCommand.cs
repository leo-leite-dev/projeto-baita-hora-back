using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Features.Companies.Responses;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record CreateCompanyServiceCommand
    : IRequest<Result<CreateCompanyServiceResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }

    public string ServiceName { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public IReadOnlyCollection<Guid> PositionIds { get; init; } = Array.Empty<Guid>();
}