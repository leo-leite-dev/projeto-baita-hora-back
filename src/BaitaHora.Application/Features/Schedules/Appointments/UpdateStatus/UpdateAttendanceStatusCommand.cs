using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Schedules.Enums;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.UpdateStatus;

public sealed record UpdateAttendanceStatusCommand
    : IRequest<Result<Unit>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid MemberId { get; init; }
    public Guid AppointmentId { get; init; }
    public AttendanceStatus AttendanceStatus { get; init; }

    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageSchedule];

}
