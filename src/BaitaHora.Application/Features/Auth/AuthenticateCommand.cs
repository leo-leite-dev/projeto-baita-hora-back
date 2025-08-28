using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Auth;

public sealed record AuthenticateCommand
    : IRequest<Result<AuthTokenResponse>>
{
    public required string Identify { get; init; }
    public required string RawPassword { get; init; }
}