using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Employees.Activate;

public sealed record ActivateMembersCommand
    : IRequest<Result<ActivateEmployeesResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public required IReadOnlyCollection<Guid> EmployeeIds { get; init; } = Array.Empty<Guid>();

    public Guid ResourceId { get; init; }

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}