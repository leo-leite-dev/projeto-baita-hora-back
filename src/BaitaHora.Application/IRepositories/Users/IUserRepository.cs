using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;

namespace BaitaHora.Application.IRepositories.Users
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct);
        Task<User?> GetByUsernameAsync(Username username, CancellationToken ct);
        Task<User?> GetByIdWithProfileAsync(Guid id, CancellationToken ct = default);
    }
}