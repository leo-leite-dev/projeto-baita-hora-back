using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record RegisterEmployeeCommand
    : IRequest<Result<RegisterEmployeeResponse>>, ITransactionalRequest, IAuthorizableRequest
{
    public required Guid CompanyId { get; init; }
    public required Guid PositionId { get; init; }
    public required CreateUserCommand Employee { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageMember];
}