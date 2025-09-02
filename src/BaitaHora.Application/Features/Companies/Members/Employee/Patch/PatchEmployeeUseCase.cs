using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Members.Employee;

public sealed class PatchEmployeeUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;
    private readonly IUserGuards _userGuards;

    public PatchEmployeeUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards,
        IUserGuards userGuards)
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
        _userGuards = userGuards;
    }

    public async Task<Result<PatchEmployeeResponse>> HandleAsync(
        PatchEmployeeCommand request, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(request.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<PatchEmployeeResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var memberRes = await _memberGuards.EnsureMemberExistsAsync(company.Id, request.EmployeeId, requireActive: true, ct);
        if (memberRes.IsFailure)
            return Result<PatchEmployeeResponse>.FromError(memberRes);

        var member = memberRes.Value!;

        var userRes = await _userGuards.EnsureUserExistsWithProfileAsync(member.UserId, ct);
        if (userRes.IsFailure)
            return Result<PatchEmployeeResponse>.FromError(userRes);

        var user = userRes.Value!;

        if (member.Role == CompanyRole.Owner)
        {
            var err = Result.Conflict(
                "Não é permitido editar o fundador (Owner) pelo endpoint de funcionários.",
                ResultCodes.Conflict.BusinessRule
            );

            return Result<PatchEmployeeResponse>.FromError(err);
        }

        PatchUserApplier.Apply(user, request.NewEmployee);

        var response = new PatchEmployeeResponse(user.Id, user.Profile.Name);
        return Result<PatchEmployeeResponse>.Created(response);
    }
}