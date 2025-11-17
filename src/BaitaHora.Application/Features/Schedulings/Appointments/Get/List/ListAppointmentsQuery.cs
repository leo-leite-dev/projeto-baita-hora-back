using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.List
{
    public sealed record ListAppointmentsQuery(DateTime? DateUtc = null)
        : IRequest<Result<ScheduleDetailsDto>>;
}