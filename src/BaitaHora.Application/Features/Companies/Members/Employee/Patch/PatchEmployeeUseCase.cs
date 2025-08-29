using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Members.Employee;

public sealed class PatchEmployeeUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _companyMemberGuards;
    private readonly IUserGuards _userGuards;

    public PatchEmployeeUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards companyMemberGuards,
        IUserGuards userGuards)
    {
        _companyGuards = companyGuards;
        _companyMemberGuards = companyMemberGuards;
        _userGuards = userGuards;
    }

    public async Task<Result<PatchEmployeeResponse>> HandleAsync(
        PatchEmployeeCommand request, CancellationToken ct)
    {
        var compRes = await _companyGuards.ExistsCompany(request.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<PatchEmployeeResponse>.FromError(compRes);

        var company = compRes.Value!;

        var memberRes = _companyMemberGuards.GetMemberOrNotFound(company, request.EmployeeId);
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

        var response = new PatchEmployeeResponse(user.Id, user.Profile.FullName);
        return Result<PatchEmployeeResponse>.Created(response);
    }
}