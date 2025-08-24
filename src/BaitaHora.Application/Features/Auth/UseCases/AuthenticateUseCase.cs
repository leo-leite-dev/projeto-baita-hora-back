using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Auth.DTOs.Responses;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Auth.UseCases;

public sealed class AuthenticateUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthenticateUseCase(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthenticateUseCase> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokenResponse>> HandleAsync(AuthenticateCommand cmd, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();


        var identifier = cmd.Identify?.Trim();
        if (string.IsNullOrWhiteSpace(identifier))
            return Result<AuthTokenResponse>.BadRequest("Informe e-mail ou username.", ResultCodes.Generic.BadRequest);

        User? user;

        if (Email.TryParse(identifier, out var email))
            user = await _userRepository.GetByEmailAsync(email, ct);

        else if (Username.TryParse(identifier, out var username))
            user = await _userRepository.GetByUsernameAsync(username, ct);

        else
            return Result<AuthTokenResponse>.BadRequest("E-mail ou username inválido.", ResultCodes.Generic.BadRequest);


        if (user is null || !_passwordService.Verify(cmd.RawPassword, user.PasswordHash))
            return Result<AuthTokenResponse>.Unauthorized("Credenciais inválidas.", ResultCodes.Auth.Unauthorized);

        if (!user.IsActive)
            return Result<AuthTokenResponse>.Forbidden("Conta desativada.", ResultCodes.Auth.Forbidden);

        var token = await _tokenService.IssueTokensAsync(
            user.Id,
            user.Username.Value,
            new[] { user.Role.ToString() },
            ct: ct
        );

        return Result<AuthTokenResponse>.Ok(token, title: "Autenticado.");
    }
}


        // if (!user.IsActive)
        //     return Result.Forbidden("Usuário desativado.");

// public Task<List<User>> GetActivesAsync(CancellationToken ct)
//     => _db.Users.Where(u => u.IsActive).ToListAsync(ct);