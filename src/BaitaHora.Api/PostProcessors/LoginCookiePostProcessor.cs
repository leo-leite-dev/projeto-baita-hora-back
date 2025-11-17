using BaitaHora.Api.Web.Cookies;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Login;
using MediatR.Pipeline;

namespace BaitaHora.Api.PostProcessors;

public sealed class LoginCookiePostProcessor
    : IRequestPostProcessor<AuthenticateCommand, Result<AuthResult>>
{
    private readonly IHttpContextAccessor _http;
    private readonly IJwtCookieFactory _cookieFactory;
    private readonly IJwtCookieWriter _cookieWriter;
    private readonly TimeProvider _timeProvider;

    public LoginCookiePostProcessor(
        IHttpContextAccessor http,
        IJwtCookieFactory cookieFactory,
        IJwtCookieWriter cookieWriter,
        TimeProvider timeProvider)
    {
        _http = http;
        _cookieFactory = cookieFactory;
        _cookieWriter = cookieWriter;
        _timeProvider = timeProvider;
    }

    public Task Process(AuthenticateCommand request, Result<AuthResult> response, CancellationToken ct)
    {
        if (!response.IsSuccess || response.Value is null)
            return Task.CompletedTask;

        var auth = response.Value;

        if (string.IsNullOrWhiteSpace(auth.AccessToken))
            return Task.CompletedTask;

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var ttl = auth.ExpiresAtUtc > now
            ? auth.ExpiresAtUtc - now
            : TimeSpan.FromDays(7);

        var http = _http.HttpContext;
        if (http is null)
            return Task.CompletedTask;

        var cookie = _cookieFactory.CreateLoginCookie(auth.AccessToken, ttl);
        _cookieWriter.Write(http.Response, cookie);

        return Task.CompletedTask;
    }
}