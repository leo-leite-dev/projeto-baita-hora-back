using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed record CreateAppointmentCommand
    : IRequest<Result<CreateAppointmentResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid MemberId { get; init; }
    public Guid CustomerId { get; init; }

    public DateTime StartsAtUtc { get; init; }

    public int DurationMinutes { get; init; } = 30;

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}