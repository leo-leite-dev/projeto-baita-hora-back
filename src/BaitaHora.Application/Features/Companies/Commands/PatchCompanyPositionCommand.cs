using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record PatchCompanyPositionCommand
    : IRequest<Result<CreateCompanyPositionResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public Guid PositionId { get; init; }
    public string PositionName { get; init; } = default!;
    public CompanyRole AccessLevel { get; init; }
    public uint RowVersion { get; init; }

    public Guid ResourceId => CompanyId;
    public IEnumerable<CompanyPermission> RequiredPermissions => [CompanyPermission.ManagePositions];
}