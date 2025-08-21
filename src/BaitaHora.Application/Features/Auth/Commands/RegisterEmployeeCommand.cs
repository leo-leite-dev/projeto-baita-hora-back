using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Users.DTOs;
using BaitaHora.Domain.Features.Companies.Enums;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record RegisterEmployeeCommand
    : IRequest<Result<RegisterEmployeeResponse>>, ITransactionalRequest
{
    public required Guid CompanyId { get; init; }
    public required UserInput User { get; init; }
    public required Guid PositionId { get; init; }
    public CompanyRole Role { get; init; } = CompanyRole.Viewer;
}