using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Feature.Customers;

public sealed record CreateCustomerCommand
    : IRequest<Result<Guid>>, IAuthorizableRequest, ITransactionalRequest
{
    public required string CustomerName { get; init; }
    public required string CustomerPhone { get; init; }
    public required string CustomerCpf { get; init; }

    public Guid ResourceId { get; init; }
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}