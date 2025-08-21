using BaitaHora.Domain.Commons.ValueObjects;
using BaitaHora.Domain.Users.Entities;
using BaitaHora.Domain.Users.ValueObjects;

namespace BaitaHora.Application.IRepositories.Users;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct);
    Task<User?> GetByUsernameAsync(Username username, CancellationToken ct);
    Task<bool> IsUserEmailTakenAsync(Email email, Guid? excludingUserId, CancellationToken ct);
    Task<bool> IsUsernameTakenAsync(Username username, Guid? excludingUserId, CancellationToken ct);

    Task AddAsync(User user, CancellationToken ct);
}