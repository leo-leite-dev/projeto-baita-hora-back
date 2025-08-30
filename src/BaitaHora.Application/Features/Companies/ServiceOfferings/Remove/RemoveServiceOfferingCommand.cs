using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed record RemoveServiceOfferingCommand
    : IRequest<Result<RemoveServiceOfferingResponse>>, ITransactionalRequest, IAuthorizableRequest
{
    public Guid CompanyId { get; init; }
    public Guid ServiceOfferingId { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.RemoveServiceOfferings];
}