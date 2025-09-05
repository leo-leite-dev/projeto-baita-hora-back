using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed record ListServiceOfferingsQuery(Guid CompanyId)
    : IRequest<Result<IReadOnlyList<ServiceOfferingDetails>>>;