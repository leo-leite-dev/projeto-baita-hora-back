using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;

public sealed record DisableServiceOfferingsCommand
    : IRequest<Result>, IAuthorizableRequest, ITransactionalRequest
{
    public IReadOnlyCollection<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId { get; init; }

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.DisableServiceOfferings];
}