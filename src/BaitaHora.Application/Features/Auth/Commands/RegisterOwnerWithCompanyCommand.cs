using BaitaHora.Application.Common;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Companies.Inputs;
using BaitaHora.Application.Features.Users.DTOs;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Commands;

public sealed record RegisterOwnerWithCompanyCommand
    : IRequest<Result<RegisterOwnerWithCompanyResponse>>, ITransactionalRequest
{
    public required UserInput User { get; init; }
    public required CompanyInput Company { get; init; }
}