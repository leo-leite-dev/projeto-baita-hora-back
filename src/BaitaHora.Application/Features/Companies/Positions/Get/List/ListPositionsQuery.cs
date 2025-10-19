using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Companies.Features.Positions.Models;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ListPositions.Get.List;

public sealed record ListPositionsQuery()
    : IRequest<Result<IReadOnlyList<PositionDetails>>>;
