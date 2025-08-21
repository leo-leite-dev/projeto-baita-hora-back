using System.Security.Claims;
using BaitaHora.Application.Auth.DTOs.Responses;

namespace BaitaHora.Application.IServices.Auth
{
    public interface ITokenService
    {
        AuthTokenResponse IssueTokens(Guid userId, string username, IEnumerable<string> roles, IDictionary<string, string>? extraClaims = null);
        AuthTokenResponse Refresh(string refreshToken);

        ClaimsPrincipal? ValidateToken(string token);
    }
}