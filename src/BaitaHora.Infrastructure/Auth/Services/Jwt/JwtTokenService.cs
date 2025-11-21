using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.Ports;
using BaitaHora.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Application.Features.Auth.Login;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace BaitaHora.Infrastructure.Services.Auth.Jwt;

public sealed class JwtTokenService : ITokenService
{
    private readonly TokenOptions _opt;
    private readonly ILoginSessionRepository _sessionRepository;
    private readonly ICompanyMemberRepository _memberRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserIdentityPort _identity;
    private readonly ILogger<JwtTokenService> _log;

    public JwtTokenService(
        IOptions<TokenOptions> opt,
        ILoginSessionRepository sessionRepository,
        ICompanyMemberRepository memberRepository,
        ICompanyRepository companyRepository,
        IUserIdentityPort identity,
        ILogger<JwtTokenService> log)
    {
        _opt = opt.Value;
        _sessionRepository = sessionRepository;
        _memberRepository = memberRepository;
        _companyRepository = companyRepository;
        _identity = identity;
        _log = log;
    }

    public async Task<AuthResult> IssueTokensAsync(
        Guid userId,
        Username username,
        IEnumerable<string> roles,
        IDictionary<string, string>? extraClaims = null,
        IEnumerable<AuthCompanySummary>? companies = null,
        CancellationToken ct = default)
    {
        var memberships = await _memberRepository.GetByUserIdAsync(userId, ct);
        var active = memberships.Where(m => m.IsActive).ToList();
        if (active.Count == 0)
            throw new InvalidOperationException("Usuário não possui empresas ativas.");

        Guid companyId;
        if (extraClaims is not null &&
            extraClaims.TryGetValue("companyId", out var cid) &&
            Guid.TryParse(cid, out var parsedCompany))
        {
            companyId = parsedCompany;
        }
        else
        {
            companyId = active
                .OrderBy(m => m.JoinedAt)
                .Select(m => m.CompanyId)
                .First();
        }

        Guid memberId;
        if (extraClaims is not null &&
            extraClaims.TryGetValue("memberId", out var mid) &&
            Guid.TryParse(mid, out var parsedMember))
        {
            memberId = parsedMember;
        }
        else
        {
            memberId = active
                .Where(m => m.CompanyId == companyId)
                .Select(m => m.Id)
                .FirstOrDefault();

            if (memberId == Guid.Empty)
                throw new InvalidOperationException("Membership não encontrado ou inativo para a empresa selecionada.");
        }

        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username.Value),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username.Value),
            new Claim("companyId", companyId.ToString()),
            new Claim("memberId", memberId.ToString())
        };

        foreach (var r in roles)
            claims.Add(new Claim(ClaimTypes.Role, r));

        if (extraClaims is not null)
        {
            foreach (var kv in extraClaims)
            {
                if (claims.Any(c => c.Type == kv.Key)) continue;
                claims.Add(new Claim(kv.Key, kv.Value));
            }
        }

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

        await _sessionRepository.AddAsync(session, ct);
        await _sessionRepository.SaveChangesAsync(ct);

        var companySummaries = companies?.ToList();
        if (companySummaries is null)
        {
            var distinctCompanyIds = active.Select(m => m.CompanyId).Distinct().ToList();
            companySummaries = new List<AuthCompanySummary>(distinctCompanyIds.Count);
            foreach (var id in distinctCompanyIds)
            {
                var company = await _companyRepository.GetByIdAsync(id, ct);
                companySummaries.Add(new AuthCompanySummary(id, company?.Name ?? string.Empty));
            }
        }

        var role = roles is not null
            ? Enum.Parse<CompanyRole>(roles.First())
            : CompanyRole.Viewer;

        return new AuthResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAtUtc: accessExpires,
            UserId: userId,
            Username: username,
            Role: role,
            Companies: companySummaries,
            MemberId: memberId
        );
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var hash = Sha256(refreshToken);

        var dto = await _sessionRepository.GetByRefreshTokenHashAsync(hash, ct);
        if (dto is null || dto.IsRevoked || dto.RefreshTokenExpiresAtUtc <= now)
            throw new SecurityTokenException("Refresh token inválido.");

        var (usernameStr, roles, isActive) = await _identity.GetIdentityAsync(dto.UserId, ct);
        if (!isActive)
            throw new SecurityTokenException("Conta desativada.");

        if (!Username.TryParse(usernameStr, out var username))
            throw new SecurityTokenException("Username inválido.");

        await _sessionRepository.InvalidateAsync(dto.Id, ct);
        await _sessionRepository.SaveChangesAsync(ct);

        var memberships = await _memberRepository.GetByUserIdAsync(dto.UserId, ct);
        var active = memberships.Where(m => m.IsActive).ToList();
        if (active.Count == 0)
            throw new SecurityTokenException("Usuário não possui empresas ativas.");

        var defaultCompanyId = active
            .OrderBy(m => m.JoinedAt)
            .Select(m => m.CompanyId)
            .First();

        var memberId = active
            .Where(m => m.CompanyId == defaultCompanyId)
            .Select(m => m.Id)
            .First();

        var extra = new Dictionary<string, string>
        {
            ["companyId"] = defaultCompanyId.ToString(),
            ["memberId"] = memberId.ToString()
        };

        var distinctCompanyIds = active.Select(m => m.CompanyId).Distinct().ToList();
        var companies = new List<AuthCompanySummary>(distinctCompanyIds.Count);
        foreach (var id in distinctCompanyIds)
        {
            var company = await _companyRepository.GetByIdAsync(id, ct);
            companies.Add(new AuthCompanySummary(id, company?.Name ?? string.Empty));
        }

        return await IssueTokensAsync(
            userId: dto.UserId,
            username: username,
            roles: roles,
            extraClaims: extra,
            companies: companies,
            ct: ct
        );
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