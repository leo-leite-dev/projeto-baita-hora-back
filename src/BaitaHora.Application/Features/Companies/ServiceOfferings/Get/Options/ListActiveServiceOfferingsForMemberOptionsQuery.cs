using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.Options;

public sealed record ListActiveServiceOfferingsForMemberOptionsQuery(
    string? Search = null,
    int Take = 20
) : IRequest<Result<IReadOnlyList<ServiceOfferingOptions>>>;
