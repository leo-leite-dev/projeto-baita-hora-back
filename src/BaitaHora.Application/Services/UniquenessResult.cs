using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Common;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.Services;

public sealed class UserUniquenessChecker : IUserUniquenessChecker
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;

    public UserUniquenessChecker(IUserRepository users, IUserProfileRepository profiles)
        => (_userRepository, _profileRepository) = (users, profiles);

    public async Task<UniquenessResult> CheckAsync(
        Email email, Username username, CPF cpf, RG? rg, Guid? excludingUserId, CancellationToken ct)
    {
        var violations = new List<UniquenessViolation>(4);

        if (await _userRepository.IsUserEmailTakenAsync(email, excludingUserId, ct))
            violations.Add(new("userEmail", "unique_violation", "E-mail já cadastrado."));

        if (await _userRepository.IsUsernameTakenAsync(username, excludingUserId, ct))
            violations.Add(new("username", "unique_violation", "Username já cadastrado."));

        if (await _profileRepository.IsCpfTakenAsync(cpf, excludingUserId, ct))
            violations.Add(new("cpf", "unique_violation", "CPF já cadastrado."));

        if (rg is not null && await _profileRepository.IsRgTakenAsync(rg.Value, excludingUserId, ct))
            violations.Add(new("rg", "unique_violation", "RG já cadastrado."));

        return violations.Count == 0
            ? UniquenessResult.Ok()
            : UniquenessResult.Fail(violations.ToArray());
    }
}