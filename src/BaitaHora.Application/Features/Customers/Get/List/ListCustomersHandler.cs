using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Customers.Get.ReadModels;
using BaitaHora.Application.IRepositories.Customers;
using MediatR;

namespace BaitaHora.Application.Features.Customers.Get.List;

public sealed class ListCustomersHandler
    : IRequestHandler<ListCustomersQuery, Result<IReadOnlyList<CustomerOptions>>>
{
    private readonly ICustomerRepository _repository;
    private readonly ICurrentCompany _currentCompany;

    public ListCustomersHandler(ICustomerRepository repository, ICurrentCompany currentCompany)
    {
        _repository = repository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<CustomerOptions>>> Handle(
        ListCustomersQuery request, CancellationToken ct)
    {
        var list = await _repository.SearchCustomersAsync(
            request.Search ?? string.Empty,
            ct
        );

        return Result<IReadOnlyList<CustomerOptions>>.Ok(list);
    }
}