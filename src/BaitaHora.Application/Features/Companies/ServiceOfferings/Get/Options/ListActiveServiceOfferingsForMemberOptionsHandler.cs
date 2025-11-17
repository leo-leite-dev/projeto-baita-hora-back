using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.Options;

public sealed class ListActiveServiceOfferingsForMemberOptionsHandler
    : IRequestHandler<ListActiveServiceOfferingsForMemberOptionsQuery, Result<IReadOnlyList<ServiceOfferingOptions>>>
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;
    private readonly ICurrentCompany _currentCompany;
    private readonly ICurrentUser _currentUser;

    public ListActiveServiceOfferingsForMemberOptionsHandler(
        ICompanyServiceOfferingRepository serviceOfferingRepository,
        ICurrentCompany currentCompany,
        ICurrentUser currentUser)
    {
        _serviceOfferingRepository = serviceOfferingRepository;
        _currentCompany = currentCompany;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ServiceOfferingOptions>>> Handle(
        ListActiveServiceOfferingsForMemberOptionsQuery request, CancellationToken ct)
    {
        if (_currentUser.MemberId is null)
            return Result<IReadOnlyList<ServiceOfferingOptions>>.Forbidden("Usuário sem vínculo de membro.");

        var items = await _serviceOfferingRepository.ListServiceOfferingActiveForMemberOptionsAsync(
            _currentCompany.Id,
            _currentUser.MemberId.Value,
            request.Search,
            request.Take,
            ct);

        return Result<IReadOnlyList<ServiceOfferingOptions>>.Ok(items);
    }
}