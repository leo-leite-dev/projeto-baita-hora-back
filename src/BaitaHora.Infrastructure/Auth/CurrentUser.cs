using System.Security.Claims;
using BaitaHora.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;
using BaitaHora.Domain.Features.Common.ValueObjects;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public bool IsAuthenticated =>
        _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var raw = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _http.HttpContext?.User?.FindFirstValue("sub");

            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }

    public Username Username
    {
        get
        {
            var raw = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                      ?? _http.HttpContext?.User?.Identity?.Name;

            return Username.TryParse(raw, out var username)
                ? username
                : Username.Empty;
        }
    }

    public string? Email =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public Guid? MemberId
    {
        get
        {
            var raw = _http.HttpContext?.User?.FindFirstValue("memberId");
            return Guid.TryParse(raw, out var id) ? id : (Guid?)null;
        }
    }
}
