using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Infrastructure.Repositories.Users;

public sealed class UserProfileRepository : GenericRepository<User>, IUserProfileRepository
{
    public UserProfileRepository(AppDbContext context) : base(context) { }

}