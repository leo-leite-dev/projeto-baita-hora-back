using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Login;

public sealed record AuthenticateCommand : IRequest<Result<AuthResult>>
{
    public required string Identify { get; init; }
    public required string RawPassword { get; init; }
    public Guid? CompanyId { get; init; }
}