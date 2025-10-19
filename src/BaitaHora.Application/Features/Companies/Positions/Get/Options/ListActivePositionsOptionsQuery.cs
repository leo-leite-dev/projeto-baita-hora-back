using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Get.Combo;

public sealed record ListActivePositionsOptionsQuery(
    string? Search = null,
    int Take = 20
) : IRequest<Result<IReadOnlyList<PositionOptions>>>;