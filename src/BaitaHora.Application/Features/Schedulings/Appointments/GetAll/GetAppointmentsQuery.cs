using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.GetAll;

public sealed record GetAppointmentsQuery(DateTime? Date)
    : IRequest<Result<IReadOnlyList<GetAppointmentsResult>>>;