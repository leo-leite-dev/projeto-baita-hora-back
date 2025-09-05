using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

public sealed class GetServiceOfferingByIdHandler
    : IRequestHandler<GetServiceOfferingByIdQuery, Result<ServiceOfferingDetails>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    public GetServiceOfferingByIdHandler(ICompanyServiceOfferingRepository serviceOfferingRepository) => _serviceOfferingRepository = serviceOfferingRepository;

    public async Task<Result<ServiceOfferingDetails>> Handle(GetServiceOfferingByIdQuery request, CancellationToken ct)
    {
        var dto = await _serviceOfferingRepository.GetByIdAsync(request.CompanyId, request.ServiceOfferingId, ct);

        return dto is null
            ? Result<ServiceOfferingDetails>.NotFound("Serviço não encontrado.")
            : Result<ServiceOfferingDetails>.Ok(dto);
    }
}