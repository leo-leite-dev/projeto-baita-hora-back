
using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Inputs;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record RegisterEmployeeCommand(RegisterEmployeeInput Input)
    : IRequest<Result<RegisterEmployeeResponse>>;