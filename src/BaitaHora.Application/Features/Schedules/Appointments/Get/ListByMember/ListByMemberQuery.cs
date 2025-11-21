using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.ListByMember;

public sealed record ListByMemberQuery(
    Guid MemberId,
    DateTime? DateUtc = null
) : IRequest<Result<ScheduleDetailsDto>>;