
using BaitaHora.Application.Auth.DTO.Responses;
using BaitaHora.Application.Auth.Inputs;
using BaitaHora.Application.Common;
using MediatR;

namespace BaitaHora.Application.Auth.Commands;

public sealed record RegisterEmployeeCommand(RegisterEmployeeInput Input)
    : IRequest<Result<RegisterEmployeeResponse>>;