using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed class ListActiveServiceOfferingsComboHandler
    : IRequestHandler<ListActiveServiceOfferingsComboQuery, Result<IReadOnlyList<ServiceOfferingComboItem>>>
{
    private readonly ICompanyServiceOfferingRepository _repo;
    public ListActiveServiceOfferingsComboHandler(ICompanyServiceOfferingRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<ServiceOfferingComboItem>>> Handle(
        ListActiveServiceOfferingsComboQuery request, CancellationToken ct)
    {
        var items = await _repo.ListActiveForComboAsync(
            request.CompanyId, request.Search, request.Take, ct);

        return Result<IReadOnlyList<ServiceOfferingComboItem>>.Ok(items);
    }
}