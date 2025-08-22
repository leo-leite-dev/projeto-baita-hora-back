using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Features.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Users.Commands;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record RegisterOwnerWithCompanyCommand
    : IRequest<Result<RegisterOwnerWithCompanyResponse>>, ITransactionalRequest
{
    public required UserCommand Owner { get; init; }
    public required CompanyCommand Company { get; init; }
}