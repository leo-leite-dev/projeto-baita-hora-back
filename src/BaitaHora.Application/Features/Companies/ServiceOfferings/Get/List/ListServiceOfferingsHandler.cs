using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed class ListServiceOfferingsHandler
    : IRequestHandler<ListServiceOfferingsQuery, Result<IReadOnlyList<ServiceOfferingDetails>>>
{
    private readonly ICompanyServiceOfferingRepository _repo;

    public ListServiceOfferingsHandler(ICompanyServiceOfferingRepository repo) => _repo = repo;

    public async Task<Result<IReadOnlyList<ServiceOfferingDetails>>> Handle(
        ListServiceOfferingsQuery request, CancellationToken ct)
    {
        var list = await _repo.ListAllByCompanyAsync(request.CompanyId, ct);

        return Result<IReadOnlyList<ServiceOfferingDetails>>.Ok(list);
    }
}