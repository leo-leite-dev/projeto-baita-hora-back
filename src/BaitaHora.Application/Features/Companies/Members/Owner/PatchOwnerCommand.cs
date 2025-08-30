using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Owner;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Domain.Permissions;
using MediatR;

public sealed record PatchOwnerCommand
    : IRequest<Result<PatchOwnerResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public required PatchUserCommand NewOwner { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.All];
    public bool RequireAllPermissions => true;
}