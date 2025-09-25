using BaitaHora.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Http;

namespace BaitaHora.Infrastructure.Web.Security;

public sealed class HttpContextCurrentCompany : ICurrentCompany
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentCompany(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool HasValue =>
        Guid.TryParse(
            _httpContextAccessor.HttpContext?.User.FindFirst("companyId")?.Value,
            out _);

    public Guid Id =>
        Guid.Parse(
            _httpContextAccessor.HttpContext?.User.FindFirst("companyId")?.Value
            ?? throw new InvalidOperationException("CompanyId não encontrado no token."));
}