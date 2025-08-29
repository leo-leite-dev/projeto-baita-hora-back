using BaitaHora.Application.Common.Authorization;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed record PatchCompanyPositionCommand
    : IRequest<Result<PatchCompanyPositionResponse>>, IAuthorizableRequest, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid PositionId { get; init; }
    
    public string? NewPositionName { get; init; } = default!;
    public CompanyRole? NewAccessLevel { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManageCompany];
}