using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Catalog.Patch;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed record PatchCompanyServiceOfferingCommand
    : IRequest<Result<PatchCompanyServiceOfferingResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid ServiceOfferingId { get; init; }

    public string? ServiceOfferingName { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }
}