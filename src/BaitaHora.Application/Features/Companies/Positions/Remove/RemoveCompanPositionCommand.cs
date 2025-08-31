using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed record RemovePositionCommand
    : IRequest<Result<RemovePositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid PositionId { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.RemovePositions];
}