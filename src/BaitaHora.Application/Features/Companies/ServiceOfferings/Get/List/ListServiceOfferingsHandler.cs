using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.List;

public sealed class ListServiceOfferingsHandler
    : IRequestHandler<ListServiceOfferingsQuery, Result<IReadOnlyList<ServiceOfferingDetails>>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListServiceOfferingsHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<ServiceOfferingDetails>>> Handle(
        ListServiceOfferingsQuery request, CancellationToken ct)
    {
        var list = await _serviceOfferingRepository.ListAllServicesByCompanyAsync(_currentCompany.Id, ct);

        return Result<IReadOnlyList<ServiceOfferingDetails>>.Ok(list);
    }
}