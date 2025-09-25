using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;

public sealed record RemoveServicesFromPositionCommand
    : IRequest<Result<RemoveServicesFromPositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid PositionId { get; init; }
    public IReadOnlyCollection<Guid> ServiceOfferingIds { get; init; } = [];

    public Guid ResourceId { get; init; }
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}