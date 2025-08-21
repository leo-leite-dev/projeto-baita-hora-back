using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Inputs;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record AuthenticateCommand(AuthenticateInput Input)
    : IRequest<Result<AuthTokenResponse>>;