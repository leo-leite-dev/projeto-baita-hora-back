using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed class CreateAppointmentCommand
    : IRequest<Result<Guid>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid MemberId { get; init; }
    public Guid CustomerId { get; init; }
    public IReadOnlyCollection<Guid> ServiceOfferingIds { get; init; } = Array.Empty<Guid>();

    public DateTime StartsAtUtc { get; init; }
    public int DurationMinutes { get; init; }

    public Guid CompanyId { get; init; }
    public Guid ResourceId => CompanyId;

    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}