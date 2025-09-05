using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed record ListActiveServiceOfferingsComboQuery(
    Guid CompanyId,
    string? Search = null,
    int Take = 20
) : IRequest<Result<IReadOnlyList<ServiceOfferingComboItem>>>;