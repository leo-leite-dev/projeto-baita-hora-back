using BaitaHora.Application.Abstractions.Auth;

namespace BaitaHora.Infrastructure.Services.Auth.Security;

public sealed class BCryptPasswordService : IPasswordService
{
    private const int WorkFactor = 12;

    public string Hash(string rawPassword)
        => BCrypt.Net.BCrypt.HashPassword(rawPassword, workFactor: WorkFactor);

    public bool Verify(string rawPassword, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(rawPassword, passwordHash);
}