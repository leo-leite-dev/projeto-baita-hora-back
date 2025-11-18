using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.NoShow;

public sealed record NoShowAppointmentCommand
    : IRequest<Result<Unit>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid MemberId { get; init; }
    public Guid AppointmentId { get; init; }

    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}