using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using BaitaHora.Domain.Permissions;
using MediatR;

public sealed record CreateServiceOfferingCommand
    : IRequest<Result<CreateServiceOfferingResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public string ServiceOfferingName { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Amount { get; init; }

    public Guid ResourceId { get; init; }
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}