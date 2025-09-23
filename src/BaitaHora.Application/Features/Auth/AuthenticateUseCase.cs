using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Auth;

public sealed class AuthenticateUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthenticateUseCase(
        IUserRepository userRepository,
        ICompanyMemberRepository companyMemberRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _companyMemberRepository = companyMemberRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthTokenResponse>> HandleAsync(AuthenticateCommand cmd, CancellationToken ct)
    {
        var user = await FindUserByIdentifyAsync(cmd.Identify, ct);
        if (user is null || !_passwordService.Verify(cmd.RawPassword, user.PasswordHash))
            return Result<AuthTokenResponse>.Unauthorized("Credenciais inválidas.");

        if (!user.IsActive)
            return Result<AuthTokenResponse>.Forbidden("Conta desativada.");

        var memberships = await _companyMemberRepository.GetByUserIdAsync(user.Id, ct);
        var active = memberships.Where(m => m.IsActive).ToList();

        if (active.Count == 0)
            return Result<AuthTokenResponse>.Forbidden("Usuário não possui empresas ativas.");

        if (active.Count == 1)
        {
            var m = active[0];

            var token = await _tokenService.IssueTokensAsync(
                user.Id,
                user.Username.Value,
                [m.Role.ToString()],
                new Dictionary<string, string> { { "companyId", m.CompanyId.ToString() } }

            );

            return Result<AuthTokenResponse>.Ok(token);
        }

        var companies = active
            .Select(m => new { m.CompanyId, Role = m.Role.ToString() })
            .ToList();

        return Result<AuthTokenResponse>
            .Ok(value: default!, title: "Selecione a empresa.")
            .WithMeta("requiresCompanySelection", true)
            .WithMeta("companies", companies);
    }

    private async Task<User?> FindUserByIdentifyAsync(string identify, CancellationToken ct)
    {
        var id = identify?.Trim();
        if (string.IsNullOrWhiteSpace(id)) return null;

        if (Email.TryParse(id, out var email))
            return await _userRepository.GetByEmailAsync(email, ct);

        if (Username.TryParse(id, out var username))
            return await _userRepository.GetByUsernameAsync(username, ct);

        return null;
    }
}