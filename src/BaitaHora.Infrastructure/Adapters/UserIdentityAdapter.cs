// using BaitaHora.Application.IRepositories;
// using BaitaHora.Application.Ports;

// namespace BaitaHora.Infrastructure.Adapters.Auth;


// public sealed class UserIdentityAdapter : IUserIdentityPort
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IUserRoleRepository _userRoles;

//     public UserIdentityAdapter(IUserRepository userRepository, IUserRoleRepository userRoles)
//     {
//         _userRepository = userRepository;
//         _userRoles = userRoles;
//     }

//     public async Task<(string Username, IEnumerable<string> Roles, bool IsActive)> GetIdentityAsync(Guid userId, CancellationToken ct)
//     {
//         var user = await _userRepository.GetByIdAsync(userId, ct);
//         if (user is null) return ("", Array.Empty<string>(), false);

//         var roles = await _userRoles.GetRoleNamesByUserIdAsync(userId, ct);
//         var username = user.Username?.Value ?? userId.ToString();
//         return (username, roles, user.IsActive);
//     }
// }