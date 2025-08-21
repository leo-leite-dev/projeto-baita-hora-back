namespace BaitaHora.Infrastructure.Persistence.Entities;

public sealed class LoginSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RefreshTokenHash { get; set; } = default!;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; }
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}