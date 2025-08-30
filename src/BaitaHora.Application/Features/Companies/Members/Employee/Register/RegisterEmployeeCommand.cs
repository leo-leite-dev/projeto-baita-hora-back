using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Register;

public sealed record RegisterEmployeeCommand
    : IRequest<Result<RegisterEmployeeResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public required Guid CompanyId { get; init; }
    public required Guid PositionId { get; init; }
    public required CreateUserCommand Employee { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}