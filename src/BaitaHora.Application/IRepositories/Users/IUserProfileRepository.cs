using BaitaHora.Domain.Users.ValueObjects;

namespace BaitaHora.Application.IRepositories.Users;

public interface IUserProfileRepository
{
    Task<bool> IsCpfTakenAsync(CPF cpf, Guid? excludingUserId, CancellationToken ct);
    Task<bool> IsRgTakenAsync(RG rg, Guid? excludingUserId, CancellationToken ct);
}
