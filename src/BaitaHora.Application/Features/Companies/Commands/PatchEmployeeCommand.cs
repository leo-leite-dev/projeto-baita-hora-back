using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record PatchEmployeeCommand
    : IRequest<Result<PatchEmployeeResponse>>, ITransactionalRequest, IAuthorizableRequest
{
    public required Guid CompanyId { get; init; }
    public required Guid EmployeeId { get; init; }

    public Guid? PositionId { get; init; }
    public bool? IsActive { get; init; }

    public PatchUserCommand? Employee { get; set; }

    public Guid ResourceId => CompanyId;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}