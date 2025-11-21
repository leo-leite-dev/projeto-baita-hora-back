using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

public sealed record ListServiceOfferingByIdQuery(Guid ServiceOfferingId)
    : IRequest<Result<ServiceOfferingEditView>>;
    