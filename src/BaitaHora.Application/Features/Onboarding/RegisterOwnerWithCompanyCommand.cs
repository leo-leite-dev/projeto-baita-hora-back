using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Users.CreateUser;
using MediatR;

namespace BaitaHora.Application.Features.Onboarding;

public sealed record RegisterOwnerWithCompanyCommand
    : IRequest<Result<RegisterOwnerWithCompanyResponse>>, ITransactionalRequest
{
    public required CreateUserCommand Owner { get; init; }
    public required CreateCompanyWithOwnerCommand Company { get; init; }
}