using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BaitaHora.Application.Ports;

namespace BaitaHora.Api.Web.Adapters;

public sealed class HttpContextUserIdentityAdapter : IUserIdentityPort
{
    private readonly IHttpContextAccessor _http;

    public HttpContextUserIdentityAdapter(IHttpContextAccessor httpContextAccessor)
        => _http = httpContextAccessor;

    public Guid GetMemberId()
    {
        var user = _http.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        var memberClaim = user.FindFirst("memberId") 
            ?? throw new UnauthorizedAccessException("Claim 'memberId' não encontrada no token.");

        if (!Guid.TryParse(memberClaim.Value, out var memberId))
            throw new UnauthorizedAccessException("Claim 'memberId' inválida.");

        return memberId;
    }

    public Task<(string Username, IEnumerable<string> Roles, bool IsActive)>
        GetIdentityAsync(Guid memberId, CancellationToken ct)
    {
        var user = _http.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        var memberClaim = user.FindFirst("member_id");
        if (memberClaim is null || !Guid.TryParse(memberClaim.Value, out var tokenMemberId) || tokenMemberId != memberId)
            throw new UnauthorizedAccessException("Identidade solicitada não corresponde ao token atual.");

        var username = user.FindFirst(ClaimTypes.Name)?.Value
                    ?? user.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value
                    ?? throw new UnauthorizedAccessException("Claim de nome de usuário não encontrada.");

        IEnumerable<string> roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

        var isActiveClaim = user.FindFirst("is_active")?.Value;
        var isActive = isActiveClaim is null ? true : bool.TryParse(isActiveClaim, out var b) && b;

        return Task.FromResult((Username: username, Roles: roles, IsActive: isActive));
    }
}