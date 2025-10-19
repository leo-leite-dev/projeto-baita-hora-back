using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Employees.Disable;

public sealed record DisableMembersCommand
    : IRequest<Result<DisableEmployeesResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public required IReadOnlyCollection<Guid> EmployeeIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId { get; init; }

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}