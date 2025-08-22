using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Auth.DTOs.Responses;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record AuthenticateCommand
    : IRequest<Result<AuthTokenResponse>>
{
    public required string Identify { get; init; }
    public required string RawPassword { get; init; }
}