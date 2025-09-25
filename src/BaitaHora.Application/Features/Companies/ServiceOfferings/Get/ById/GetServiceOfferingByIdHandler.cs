using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

public sealed class GetServiceOfferingByIdHandler
    : IRequestHandler<GetServiceOfferingByIdQuery, Result<ServiceOfferingDetails>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;

    public GetServiceOfferingByIdHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<ServiceOfferingDetails>> Handle(GetServiceOfferingByIdQuery request, CancellationToken ct)
    {
        var dto = await _serviceOfferingRepository.GetByIdAsync(_currentCompany.Id, request.ServiceOfferingId, ct);

        return dto is null
            ? Result<ServiceOfferingDetails>.NotFound("Serviço não encontrado.")
            : Result<ServiceOfferingDetails>.Ok(dto);
    }
}