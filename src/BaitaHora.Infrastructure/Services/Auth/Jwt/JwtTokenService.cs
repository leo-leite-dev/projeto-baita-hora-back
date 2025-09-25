using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Abstractions.Companies;
using BaitaHora.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BaitaHora.Application.Features.Auth;
using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.Ports;

namespace BaitaHora.Infrastructure.Services.Auth.Jwt;

public sealed class JwtTokenService : ITokenService
{
    private readonly TokenOptions _opt;
    private readonly ILoginSessionRepository _sessions;
    private readonly IUserIdentityPort _identity;
    private readonly ILogger<JwtTokenService> _log;

    public JwtTokenService(
        IOptions<TokenOptions> opt,
        ILoginSessionRepository sessions,
        IUserIdentityPort identity,
        ILogger<JwtTokenService> log)
    {
        _opt = opt.Value;
        _sessions = sessions;
        _identity = identity;
        _log = log;
    }

    public async Task<AuthTokenResponse> IssueTokensAsync(
        Guid userId,
        string username,
        IEnumerable<string> roles,
        IDictionary<string, string>? extraClaims = null,
        IEnumerable<CompanySummary>? companies = null,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        if (extraClaims is not null)
            foreach (var kv in extraClaims)
                claims.Add(new Claim(kv.Key, kv.Value));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var jwt = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExpires,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = GenerateSecureToken();
        var refreshHash = Sha256(refreshToken);
        var refreshExpires = now.AddDays(_opt.RefreshTokenDays);

        var session = new LoginSessionDto(
            Id: Guid.NewGuid(),
            UserId: userId,
            RefreshTokenHash: refreshHash,
            RefreshTokenExpiresAtUtc: refreshExpires,
            IsRevoked: false,
            Ip: string.Empty,
            UserAgent: string.Empty,
            CreatedAtUtc: now
        );

        await _sessions.AddAsync(session, ct);
        await _sessions.SaveChangesAsync(ct);

        return new AuthTokenResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAtUtc: accessExpires,
            UserId: userId,
            Username: username,
            Roles: roles,
            Companies: companies ?? Array.Empty<CompanySummary>()
        );
    }

    public async Task<AuthTokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var hash = Sha256(refreshToken);

        var dto = await _sessions.GetByRefreshTokenHashAsync(hash, ct);
        if (dto is null || dto.IsRevoked || dto.RefreshTokenExpiresAtUtc <= now)
            throw new SecurityTokenException("Refresh token inválido.");

        var (username, roles, isActive) = await _identity.GetIdentityAsync(dto.UserId, ct);
        if (!isActive)
            throw new SecurityTokenException("Conta desativada.");

        await _sessions.InvalidateAsync(dto.Id, ct);
        await _sessions.SaveChangesAsync(ct);

        return await IssueTokensAsync(dto.UserId, username, roles, ct: ct);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_opt.Secret);

            return tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _opt.Issuer,
                ValidateAudience = true,
                ValidAudience = _opt.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            }, out _);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "[JwtTokenService] Falha ao validar token.");
            return null;
        }
    }

    private static string GenerateSecureToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash);
    }
}