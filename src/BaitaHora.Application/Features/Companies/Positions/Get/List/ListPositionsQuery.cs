using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Positions.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ListPositions.Get.List;

public sealed record ListPositionsQuery()
    : IRequest<Result<IReadOnlyList<PositionDetails>>>;
