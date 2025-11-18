using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Get.ById;

public sealed record GetScheduleByUserQuery(DateTime? FromUtc = null, DateTime? ToUtc = null)
    : IRequest<Result<ScheduleDetailsDto>>;