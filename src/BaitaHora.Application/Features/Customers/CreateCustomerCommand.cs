using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Customers.Create;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Feature.Customers;

public sealed record CreateCustomerCommand
    : IRequest<Result<CreateCustomerResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public string CustomerName { get; init; } = default!;
    public string CustomerPhone { get; init; } = default!;
    public string CustomerCpf { get; init; } = default!;

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}