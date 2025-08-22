using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Domain.Features.Companies.Enums;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record RegisterCompanyPositionCommand
    : IRequest<Result<CompanyPositionResponse>>, ITransactionalRequest
{
    public Guid CompanyId { get; init; }
    public string PositionName { get; init; } = default!;
    public CompanyRole AccessLevel { get; init; }

    public Guid ResourceId => CompanyId;
    public string[] RequiredPermissions => new[] { "ManagePositions" };
}