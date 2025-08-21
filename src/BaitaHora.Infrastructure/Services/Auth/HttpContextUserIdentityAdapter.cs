using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using BaitaHora.Application.Ports;
using Microsoft.AspNetCore.Http;

public sealed class HttpContextUserIdentityAdapter : IUserIdentityPort
{
    private readonly IHttpContextAccessor _http;

    public HttpContextUserIdentityAdapter(IHttpContextAccessor httpContextAccessor)
        => _http = httpContextAccessor;

    public Guid GetUserId()
    {
        var user = _http.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                   ?? user.FindFirst(JwtRegisteredClaimNames.Sub)
                   ?? throw new UnauthorizedAccessException("Claim de usuário não encontrada.");

        if (!Guid.TryParse(idClaim.Value, out var userId))
            throw new UnauthorizedAccessException("Claim de usuário inválida.");

        return userId;
    }

    public Task<(string Username, IEnumerable<string> Roles, bool IsActive)>
        GetIdentityAsync(Guid userId, CancellationToken ct)
    {
        var user = _http.HttpContext?.User
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        var sub = user.FindFirst(ClaimTypes.NameIdentifier)
                   ?? user.FindFirst(JwtRegisteredClaimNames.Sub);
        if (sub is null || !Guid.TryParse(sub.Value, out var tokenUserId) || tokenUserId != userId)
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
