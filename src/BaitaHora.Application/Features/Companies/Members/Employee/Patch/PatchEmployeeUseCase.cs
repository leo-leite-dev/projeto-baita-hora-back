using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Domain.Features.Companies.Enums;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee;

public sealed class PatchEmployeeUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;
    private readonly IUserGuards _userGuards;
    private readonly ICurrentCompany _currentCompany;

    public PatchEmployeeUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards,
        IUserGuards userGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
        _userGuards = userGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<Unit>> HandleAsync(
        PatchEmployeeCommand request,
        CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var companyRes = await _companyGuards.GetWithPositionsAndMembers(companyId, ct);
        if (companyRes.IsFailure)
            return Result<Unit>.FromError(companyRes);

        var company = companyRes.Value!;

        var memberRes = _memberGuards.ValidateMember(company, request.MemberId, requireActive: true);
        if (memberRes.IsFailure)
            return Result<Unit>.FromError(memberRes);

        var member = memberRes.Value!;

        var userRes = await _userGuards.EnsureUserExistsWithProfileAsync(member.UserId, ct);
        if (userRes.IsFailure)
            return Result<Unit>.FromError(userRes);

        var user = userRes.Value!;

        if (member.Role == CompanyRole.Owner)
        {
            var err = Result.Conflict(
                "Não é permitido editar o fundador (Owner) pelo endpoint de funcionários.",
                ResultCodes.Conflict.BusinessRule
            );

            return Result<Unit>.FromError(err);
        }

        PatchUserApplier.Apply(user, request.NewMember);

        return Result<Unit>.NoContent();
    }
}