using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed record CreatePositionCommand
    : IRequest<Result<CreatePositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public string PositionName { get; init; } = default!;
    public CompanyRole AccessLevel { get; init; }

    public IEnumerable<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId { get; init; }
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}