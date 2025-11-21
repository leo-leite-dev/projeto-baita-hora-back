using System.Security.Claims;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace BaitaHora.Api.Controllers.Auth;

public sealed class HttpCurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public Username Username { get; }
    public bool IsAuthenticated { get; }
    public string? Email { get; }
    public Guid? MemberId { get; }

    public HttpCurrentUser(IHttpContextAccessor http)
    {
        var user = http.HttpContext?.User
            ?? throw new InvalidOperationException("Sem HttpContext/User.");

        IsAuthenticated = user.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        var idStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("sub")?.Value
                    ?? throw new UnauthorizedAccessException("Token sem userId.");

        if (!Guid.TryParse(idStr, out var id))
            throw new UnauthorizedAccessException("userId inválido no token.");

        var nameStr = user.FindFirst(ClaimTypes.Name)?.Value
                      ?? user.Identity?.Name
                      ?? throw new UnauthorizedAccessException("Username ausente no token.");

        if (!Username.TryParse(nameStr, out var username))
            throw new UnauthorizedAccessException("Username inválido no token.");

        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        var memberStr = user.FindFirst("memberId")?.Value;
        Guid? memberId = null;
        if (Guid.TryParse(memberStr, out var parsedMember))
            memberId = parsedMember;

        UserId = id;
        Username = username;
        Email = email;
        MemberId = memberId;
    }
}