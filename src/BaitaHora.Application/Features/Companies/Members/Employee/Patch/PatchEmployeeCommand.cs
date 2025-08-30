using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Domain.Permissions;
using MediatR;

public sealed record PatchEmployeeCommand
    : IRequest<Result<PatchEmployeeResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid EmployeeId { get; init; }
    public required PatchUserCommand NewEmployee { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}