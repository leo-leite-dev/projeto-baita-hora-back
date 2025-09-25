using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.List;

public sealed record ListServiceOfferingsQuery()
    : IRequest<Result<IReadOnlyList<ServiceOfferingDetails>>>;
