using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Responses;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Users.Commands;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record RegisterOwnerWithCompanyCommand
    : IRequest<Result<RegisterOwnerWithCompanyResponse>>, ITransactionalRequest
{
    public required CreateUserCommand Owner { get; init; }
    public required CreateCompanyCommand Company { get; init; }
}