using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Customers.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Customers.Get.List;

public sealed record ListCustomersQuery(string? Search = null)
    : IRequest<Result<IReadOnlyList<CustomerOptions>>>;
