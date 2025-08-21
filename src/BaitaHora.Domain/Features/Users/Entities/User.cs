using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Validators;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Users.Entities;

public sealed class User : Entity
{
    public Username Username { get; private set; }
    public Email UserEmail { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;
    public CompanyRole Role { get; private set; }

    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }

    public Guid ProfileId { get; private set; }
    public UserProfile Profile { get; private set; } = null!;

    private User() { }

    public static User Create(Email UserEmail, Username username, string rawPassword, UserProfile profile, Func<string, string> hashFunction)
    {
        if (profile is null) throw new UserException("Perfil do usuário é obrigatório.");

        var user = new User();
        user.SetEmail(UserEmail);
        user.SetUsername(username);

        user.Profile = profile;
        user.ProfileId = profile.Id;
        user.IsActive = true;

        user.SetPassword(rawPassword, hashFunction);
        return user;
    }

    public void SetRole(CompanyRole newRole)
    {
        if (newRole == CompanyRole.Unknown)
            throw new UserException("Role inválida.");
        Role = newRole;
        Touch();
    }

    public bool SetEmail(Email newEmail)
    {
        if (UserEmail.Equals(newEmail)) return false;
        UserEmail = newEmail;
        Touch();
        return true;
    }

    public bool SetUsername(Username newUsername)
    {
        if (Username.Equals(newUsername)) return false;
        Username = newUsername;
        Touch();
        return true;
    }

    public void SetPassword(string rawPassword, Func<string, string> hashFunction)
    {
        if (hashFunction is null) throw new ArgumentNullException(nameof(hashFunction));
        if (string.IsNullOrWhiteSpace(rawPassword))
            throw new UserException("Senha é obrigatória.");

        PasswordValidator.EnsureStrength(rawPassword);

        PasswordHash = hashFunction(rawPassword);

        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;

        Touch();
    }

    public void ChangePassword(
        string currentRawPassword,
        string newRawPassword,
        Func<string, bool> verifyPassword,
        Func<string, string> hashFunction)
    {
        if (verifyPassword is null) throw new ArgumentNullException(nameof(verifyPassword));
        if (hashFunction is null) throw new ArgumentNullException(nameof(hashFunction));

        if (!IsActive)
            throw new UserException("Usuário inativo não pode alterar senha.");

        if (string.IsNullOrWhiteSpace(currentRawPassword))
            throw new UserException("Senha atual é obrigatória.");
        if (!verifyPassword(currentRawPassword))
            throw new UserException("A senha atual está incorreta.");

        if (string.IsNullOrWhiteSpace(newRawPassword))
            throw new UserException("Nova senha é obrigatória.");

        PasswordValidator.EnsureStrength(newRawPassword);

        var newHash = hashFunction(newRawPassword);
        if (newHash == PasswordHash)
            throw new UserException("A nova senha deve ser diferente da atual.");

        PasswordHash = newHash;

        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;

        Touch();
    }

    public void GeneratePasswordResetToken(Func<string> tokenGenerator, TimeSpan duration)
    {
        if (tokenGenerator is null) throw new ArgumentNullException(nameof(tokenGenerator));
        if (duration <= TimeSpan.Zero) throw new UserException("Duração inválida para o token.");

        PasswordResetToken = tokenGenerator();
        PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(duration);

        Touch();
    }

    public bool HasActiveResetRequest(DateTime? now = null)
    {
        var t = now ?? DateTime.UtcNow;
        return PasswordResetToken is not null
            && PasswordResetTokenExpiresAt is not null
            && PasswordResetTokenExpiresAt >= t;
    }

    public void ResetPasswordWithToken(string token, string newRawPassword, Func<string, string> hashFunction)
    {
        if (hashFunction is null) throw new ArgumentNullException(nameof(hashFunction));
        if (PasswordResetToken is null || PasswordResetTokenExpiresAt is null)
            throw new UserException("Nenhuma solicitação de recuperação de senha foi feita.");
        if (PasswordResetTokenExpiresAt < DateTime.UtcNow)
            throw new UserException("Token expirado.");
        if (!string.Equals(PasswordResetToken, token, StringComparison.Ordinal))
            throw new UserException("Token inválido.");

        SetPassword(newRawPassword, hashFunction);
    }
}