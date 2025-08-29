using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Users.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;


public sealed class UserGuards : IUserGuards
{
    private readonly IUserRepository _userRepository;

    public UserGuards(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> EnsureUserExistsWithProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdWithProfileAsync(userId, ct);
        if (user is null)
            return Result<User>.NotFound("Usuário não encontrado.");

        return Result<User>.Ok(user);
    }
}