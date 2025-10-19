using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Companies.Features.Members.Promotion;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.ChangePosition;

public sealed record ChangeMemberPositionCommand(
    Guid MemberId,
    Guid NewPositionId,
    bool AlignRoleToPosition
) : IRequest<Result<ChangeMemberPositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid ResourceId { get; init; }

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}