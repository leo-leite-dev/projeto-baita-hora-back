using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Users.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface IUserGuards
{
    Task<Result<User>> EnsureUserExistsWithProfileAsync(Guid userId, CancellationToken ct);
}