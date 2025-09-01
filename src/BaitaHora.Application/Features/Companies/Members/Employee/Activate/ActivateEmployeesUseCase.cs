using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Employees.Activate;

public sealed class ActivateEmployeesUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;

    public ActivateEmployeesUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards)
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
    }

    public async Task<Result<ActivateEmployeesResponse>> HandleAsync(
        ActivateEmployeesCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivateEmployeesResponse>.FromError(companyRes);

        var memberGuardRes = await _memberGuards.ValidateMembersForActivation(cmd.CompanyId, cmd.EmployeeIds, ct);
        if (memberGuardRes.IsFailure)
            return Result<ActivateEmployeesResponse>.FromError(memberGuardRes);

        foreach (var pos in memberGuardRes.Value!)
            pos.Activate();

        var activatedIds = memberGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<ActivateEmployeesResponse>.Ok(new(activatedIds));
    }
}