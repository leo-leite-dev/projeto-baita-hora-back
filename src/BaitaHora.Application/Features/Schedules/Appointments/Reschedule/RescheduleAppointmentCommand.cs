using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.Reschedule;

public sealed record RescheduleAppointmentCommand
    : IRequest<Result<RescheduleAppointmentResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid MemberId { get; init; }
    public Guid AppointmentId { get; init; }

    public DateTime NewStartsAtUtc { get; init; }
    public int NewDurationMinutes { get; init; } = 30;

    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageSchedule];
}
