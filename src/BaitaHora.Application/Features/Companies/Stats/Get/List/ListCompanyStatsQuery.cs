using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Stats.Get.List;

public sealed record ListCompanyStatsQuery(DateTime? DateUtc = null)
    : IRequest<Result<CompanyStatsDto>>;
