using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed record DisablePositionsCommand
    : IRequest<Result<DisablePositionsResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public IReadOnlyCollection<Guid> PositionIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId { get; init; }

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}