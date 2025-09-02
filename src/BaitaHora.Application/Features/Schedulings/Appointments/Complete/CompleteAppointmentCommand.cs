using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedules.CompleteAppointment;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Complete;

public sealed record CompleteAppointmentCommand
    : IRequest<Result<CompleteAppointmentResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid MemberId { get; init; }
    public Guid AppointmentId { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}