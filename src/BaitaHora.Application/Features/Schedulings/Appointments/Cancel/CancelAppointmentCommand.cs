using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Cancel;

public sealed record CancelAppointmentCommand
     : IRequest<Result<Unit>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid MemberId { get; init; }
    public Guid AppointmentId { get; init; }
    
    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}