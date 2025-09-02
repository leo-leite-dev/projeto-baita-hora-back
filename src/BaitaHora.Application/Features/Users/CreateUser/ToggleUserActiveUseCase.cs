// using BaitaHora.Application.Common.Results;
// using BaitaHora.Application.IRepositories;
// using BaitaHora.Domain.Features.Users.Entities;

// namespace BaitaHora.Application.Features.Users.CreateUser;

// public sealed class ToggleUserActiveUseCase
// {
//     private readonly IGenericRepository<User> _userRepository;

//     public ToggleUserActiveUseCase(IGenericRepository<User> users) => _userRepository = users;

//     public async Task<Result> HandleAsync(Guid userId, bool isActive, CancellationToken ct)
//     {
//         var user = await _userRepository.GetByIdAsync(userId, ct);
//         if (user is null) return Result.NotFound("Usuário não encontrado.");

//         var changed = isActive ? user.Activate() : user.Deactivate();
//         if (!changed) return Result.NoContent();

//         return Result.Ok();
//     }
// }