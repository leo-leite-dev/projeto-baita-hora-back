using System.Security.Claims;
using BaitaHora.Application.Features.Auth.DTOs.Responses;

namespace BaitaHora.Application.IServices.Auth
{
    public interface ITokenService
    {
        Task<AuthTokenResponse> IssueTokensAsync(Guid userId, string username, IEnumerable<string> roles, IDictionary<string, string>? extraClaims = null, CancellationToken ct = default);
        Task<AuthTokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
        ClaimsPrincipal? ValidateToken(string token);
    }
}