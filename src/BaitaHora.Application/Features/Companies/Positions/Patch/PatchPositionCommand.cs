using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed record PatchPositionCommand
    : IRequest<Result<Unit>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid PositionId { get; init; }

    public string? PositionName { get; init; } = default!;
    public CompanyRole? AccessLevel { get; init; }

    public IEnumerable<Guid>? SetServiceOfferingIds { get; init; }

    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}