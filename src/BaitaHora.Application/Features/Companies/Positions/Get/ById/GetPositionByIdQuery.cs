using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Companies.Features.Positions.Models;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Get.ById;

public sealed record GetPositionByIdQuery(Guid PositionId)
    : IRequest<Result<PositionEditView>>;