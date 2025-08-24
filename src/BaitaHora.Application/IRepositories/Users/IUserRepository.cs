using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.IRepositories.Users
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken ct);
        Task<User?> GetByUsernameAsync(Username username, CancellationToken ct);
    }
}