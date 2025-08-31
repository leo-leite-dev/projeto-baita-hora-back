using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Enable;

public sealed record ActivateServiceOfferingsCommand
    : IRequest<Result<ActivateServiceOfferingsResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public IReadOnlyCollection<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}