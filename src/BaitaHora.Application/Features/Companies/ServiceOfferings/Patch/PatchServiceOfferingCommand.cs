using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed record PatchServiceOfferingCommand
    : IRequest<Result<PatchServiceOfferingResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid ServiceOfferingId { get; init; }

    public string? ServiceOfferingName { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}