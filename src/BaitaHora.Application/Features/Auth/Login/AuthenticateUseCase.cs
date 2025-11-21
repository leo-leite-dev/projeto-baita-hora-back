using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Auth.Login;

public sealed class AuthenticateUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthenticateUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        ICompanyMemberRepository companyMemberRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResult>> HandleAsync(AuthenticateCommand cmd, CancellationToken ct)
    {
        var user = await FindUserByIdentifyAsync(cmd.Identify, ct);
        if (user is null || !_passwordService.Verify(cmd.RawPassword, user.PasswordHash))
            return Result<AuthResult>.Unauthorized("Credenciais inválidas.");

        if (!user.IsActive)
            return Result<AuthResult>.Forbidden("Conta desativada.");

        var memberships = await _companyMemberRepository.GetByUserIdAsync(user.Id, ct);
        var active = memberships.Where(m => m.IsActive).ToList();

        if (active.Count == 0)
            return Result<AuthResult>.Forbidden("Usuário não possui empresas ativas.");

        if (active.Count == 1)
        {
            var company = active.First();
            var companyEntity = await _companyRepository.GetByIdAsync(company.CompanyId, ct);

            var token = await _tokenService.IssueTokensAsync(
                user.Id,
                user.Username,
                new[] { company.Role.ToString() },
                new Dictionary<string, string>
                {
                    ["companyId"] = company.CompanyId.ToString(),
                    ["memberId"] = company.Id.ToString()
                }
            );

            var result = new AuthResult(
                token.AccessToken,
                token.RefreshToken,
                token.ExpiresAtUtc,
                user.Id,
                user.Username,
                company.Role,
                new List<AuthCompanySummary> {
                    new(company.CompanyId, companyEntity?.Name ?? string.Empty)
                },
                MemberId: company.Id
            );

            return Result<AuthResult>.Ok(result);
        }

        if (cmd.CompanyId is Guid chosen && chosen != Guid.Empty)
        {
            var membership = active.FirstOrDefault(m => m.CompanyId == chosen);
            if (membership is null || !membership.IsActive)
                return Result<AuthResult>.Forbidden("Usuário não pertence a esta empresa.");

            var companyEntity = await _companyRepository.GetByIdAsync(chosen, ct);

            var token = await _tokenService.IssueTokensAsync(
                user.Id,
                user.Username,
                new[] { membership.Role.ToString() },
                new Dictionary<string, string>
                {
                    ["companyId"] = membership.CompanyId.ToString(),
                    ["memberId"] = membership.Id.ToString()
                }
            );

            var ok = new AuthResult(
                token.AccessToken,
                token.RefreshToken,
                token.ExpiresAtUtc,
                user.Id,
                user.Username,
                 membership.Role,
                new List<AuthCompanySummary>
                {
                    new(chosen, companyEntity?.Name ?? string.Empty)
                }
            );

            return Result<AuthResult>.Ok(ok);
        }

        var companySummaries = new List<AuthCompanySummary>();
        foreach (var member in active)
        {
            var companyEntity = await _companyRepository.GetByIdAsync(member.CompanyId, ct);
            companySummaries.Add(new AuthCompanySummary(member.CompanyId, companyEntity?.Name ?? string.Empty));
        }

        var multiResult = new AuthResult(
            AccessToken: string.Empty,
            RefreshToken: string.Empty,
            ExpiresAtUtc: DateTime.UtcNow,
            UserId: user.Id,
            Username: user.Username,
            Role: CompanyRole.Viewer,  
            Companies: companySummaries
        );

        return Result<AuthResult>.Ok(multiResult);
    }

    private async Task<User?> FindUserByIdentifyAsync(string identify, CancellationToken ct)
    {
        var id = identify?.Trim();
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (Email.TryParse(id, out var email))
            return await _userRepository.GetByEmailAsync(email, ct);

        if (Username.TryParse(id, out var username))
            return await _userRepository.GetByUsernameAsync(username, ct);

        return null;
    }
}