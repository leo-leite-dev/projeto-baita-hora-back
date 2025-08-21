namespace BaitaHora.Application.IRepositories.Users;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken ct = default);
}