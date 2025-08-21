using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Inputs;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Auth.UseCases;

public sealed class AuthenticateUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ISessionService _sessions;
    private readonly ILogger<AuthenticateUseCase> _logger;

    public AuthenticateUseCase(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ISessionService sessions,
        ILogger<AuthenticateUseCase> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _sessions = sessions;
        _logger = logger;
    }

    public async Task<Result<AuthTokenResponse>> HandleAsync(AuthenticateInput input, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var identifier = input.Identify?.Trim();
        if (string.IsNullOrWhiteSpace(identifier))
            return Result<AuthTokenResponse>.BadRequest("Informe e-mail ou username.", ResultCodes.Generic.BadRequest);

        User? user;

        if (Email.TryParse(identifier, out var email))
            user = await _userRepository.GetByEmailAsync(email, ct);

        else if (Username.TryParse(identifier, out var username))
            user = await _userRepository.GetByUsernameAsync(username, ct);

        else
            return Result<AuthTokenResponse>.BadRequest("E-mail ou username inválido.", ResultCodes.Generic.BadRequest);


        if (user is null || !_passwordService.Verify(input.Password, user.PasswordHash))
            return Result<AuthTokenResponse>.Unauthorized("Credenciais inválidas.", ResultCodes.Auth.Unauthorized);

        if (!user.IsActive)
            return Result<AuthTokenResponse>.Forbidden("Conta desativada.", ResultCodes.Auth.Forbidden);

        var token = _tokenService.IssueTokens(
            user.Id,
            user.Username.Value,
            [user.Role.ToString()]
        );

        try
        {
            await _sessions.RegisterLoginAsync(
                user.Id,
                input.Ip ?? string.Empty,
                input.UserAgent ?? string.Empty,
                ct
            );
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao registrar sessão de login para o usuário {UserId}", user.Id);
        }

        return Result<AuthTokenResponse>.Ok(token, title: "Autenticado.");
    }
}