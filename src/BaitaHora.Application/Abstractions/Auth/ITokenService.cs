using System.Security.Claims;
using BaitaHora.Application.Abstractions.Companies;
using BaitaHora.Application.Features.Auth;

namespace BaitaHora.Application.Abstractions.Auth;

public interface ITokenService
{
    Task<AuthTokenResponse> IssueTokensAsync(Guid userId, string username, IEnumerable<string> roles, IDictionary<string, string>? extraClaims = null, IEnumerable<CompanySummary>? companies = null, CancellationToken ct = default);
    Task<AuthTokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
    ClaimsPrincipal? ValidateToken(string token);
}