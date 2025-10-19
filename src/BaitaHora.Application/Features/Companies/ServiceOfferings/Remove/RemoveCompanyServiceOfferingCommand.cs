using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed record RemoveServiceOfferingCommand
    : IRequest<Result>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid ServiceOfferingId { get; init; }

    public Guid ResourceId { get; init; }
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.RemoveServiceOfferings];
}