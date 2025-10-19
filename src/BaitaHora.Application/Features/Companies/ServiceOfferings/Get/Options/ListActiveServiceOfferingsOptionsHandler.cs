using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.Combo;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.Options;

public sealed class ListActiveServiceOfferingsOptionsHandler
    : IRequestHandler<ListActiveServiceOfferingsOptionsQuery, Result<IReadOnlyList<ServiceOfferingOptions>>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListActiveServiceOfferingsOptionsHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<ServiceOfferingOptions>>> Handle(
        ListActiveServiceOfferingsOptionsQuery request, CancellationToken ct)
    {
        var items = await _serviceOfferingRepository.ListServiceOfferingActiveForOptionsAsync(
            _currentCompany.Id, request.Search, request.Take, ct);

        return Result<IReadOnlyList<ServiceOfferingOptions>>.Ok(items);
    }
}
