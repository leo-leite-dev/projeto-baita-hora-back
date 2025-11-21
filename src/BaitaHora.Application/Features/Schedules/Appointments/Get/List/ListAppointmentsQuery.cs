using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.List
{
    public sealed record ListAppointmentsQuery(DateTime? DateUtc = null)
        : IRequest<Result<ScheduleDetailsDto>>;
}