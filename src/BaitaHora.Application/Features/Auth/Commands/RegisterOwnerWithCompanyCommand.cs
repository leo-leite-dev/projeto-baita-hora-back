using BaitaHora.Application.Auth.DTO.Responses;
using BaitaHora.Application.Common;
using BaitaHora.Application.Companies.Inputs;
using BaitaHora.Application.Users.DTOs;
using MediatR;

namespace BaitaHora.Application.Auth.Commands;

public sealed record RegisterOwnerWithCompanyCommand
    : IRequest<Result<RegisterOwnerWithCompanyResponse>>
{
    public required UserInput User { get; init; }
    public required CompanyInput Company { get; init; }
}
