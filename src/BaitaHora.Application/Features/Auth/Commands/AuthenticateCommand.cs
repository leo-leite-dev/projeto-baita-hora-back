using BaitaHora.Application.Auth.DTOs.Responses;
using BaitaHora.Application.Auth.Inputs;
using BaitaHora.Application.Common;
using MediatR;

namespace BaitaHora.Application.Auth.Commands;

public sealed record AuthenticateCommand(AuthenticateInput Input)
    : IRequest<Result<AuthTokenResponse>>;
