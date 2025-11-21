using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

public sealed class ListServiceOfferingByIdHandler
    : IRequestHandler<ListServiceOfferingByIdQuery, Result<ServiceOfferingEditView>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListServiceOfferingByIdHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<ServiceOfferingEditView>> Handle(ListServiceOfferingByIdQuery request, CancellationToken ct)
    {
        var dto = await _serviceOfferingRepository.GetByServiceOfferingIdAsync(_currentCompany.Id, request.ServiceOfferingId, ct);

        return dto is null
            ? Result<ServiceOfferingEditView>.NotFound("Serviço não encontrado.")
            : Result<ServiceOfferingEditView>.Ok(dto);
    }
}