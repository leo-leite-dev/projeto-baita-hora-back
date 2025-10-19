using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Get.ByUserId;

public sealed record GetMemberAdminEditByUserIdQuery(Guid UserId)
    : IRequest<Result<MemberAdminEditView>>, IAuthorizableRequest
{
    public Guid ResourceId => Guid.Empty;

    public IEnumerable<CompanyPermission> RequiredPermissions
        => [CompanyPermission.ManageSensitiveData];
}