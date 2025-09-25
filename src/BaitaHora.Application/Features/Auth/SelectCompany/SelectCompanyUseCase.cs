using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Login;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Auth.SelectCompany;

public sealed class SelectCompanyUseCase
{
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ITokenService _tokenService;

    public SelectCompanyUseCase(
        ICompanyMemberRepository companyMemberRepository,
        ICompanyRepository companyRepository,
        ITokenService tokenService)
    {
        _companyMemberRepository = companyMemberRepository;
        _companyRepository = companyRepository;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResult>> HandleAsync(
        Guid userId,
        Username username,
        SelectCompanyCommand cmd,
        CancellationToken ct)
    {
        var membership = await _companyMemberRepository.GetMemberAsync(userId, cmd.CompanyId, ct);
        if (membership is null || !membership.IsActive)
            return Result<AuthResult>.Forbidden("Usuário não pertence a esta empresa.");

        var company = await _companyRepository.GetByIdAsync(cmd.CompanyId, ct);
        var companies = new List<AuthCompanySummary>
        {
            new(cmd.CompanyId, company?.Name ?? string.Empty)
        };

        var token = await _tokenService.IssueTokensAsync(
            userId,
            username,
            new[] { membership.Role.ToString() },
            new Dictionary<string, string> { ["companyId"] = cmd.CompanyId.ToString() },
            companies,
            ct
        );

        return Result<AuthResult>.Ok(token);
    }
}