using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee;

public sealed record PatchEmployeeCommand
    : IRequest<Result<PatchEmployeeResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public required Guid MemberId { get; init; }
    public required PatchUserCommand NewMember { get; init; }

    public Guid ResourceId => Guid.Empty;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}