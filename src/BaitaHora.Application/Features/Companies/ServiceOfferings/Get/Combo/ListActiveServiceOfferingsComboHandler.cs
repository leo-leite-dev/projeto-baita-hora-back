using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.Combo;

public sealed class ListActiveServiceOfferingsComboHandler
    : IRequestHandler<ListActiveServiceOfferingsComboQuery, Result<IReadOnlyList<ServiceOfferingComboItem>>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListActiveServiceOfferingsComboHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<ServiceOfferingComboItem>>> Handle(
        ListActiveServiceOfferingsComboQuery request, CancellationToken ct)
    {
        var items = await _serviceOfferingRepository.ListActiveForComboAsync(
            _currentCompany.Id, request.Search, request.Take, ct);

        return Result<IReadOnlyList<ServiceOfferingComboItem>>.Ok(items);
    }
}