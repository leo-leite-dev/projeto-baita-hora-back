using System.Security.Claims;
using BaitaHora.Application.Features.Auth.Login;

namespace BaitaHora.Application.Abstractions.Auth;

public interface ITokenService
{
    Task<AuthResult> IssueTokensAsync(Guid userId, Username username, IEnumerable<string> roles, IDictionary<string, string>? extraClaims = null, IEnumerable<AuthCompanySummary>? companies = null, CancellationToken ct = default);
    Task<AuthResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    ClaimsPrincipal? ValidateToken(string token);
}