using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;

public sealed record RemoveServicesFromPositionCommand
    : IRequest<Result<RemoveServicesFromPositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid PositionId { get; init; }
    public IReadOnlyCollection<Guid> ServiceOfferingIds { get; init; } = [];

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}