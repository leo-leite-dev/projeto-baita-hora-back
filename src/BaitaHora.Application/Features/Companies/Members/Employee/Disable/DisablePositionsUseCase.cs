using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Employees.Disable;

public sealed class DisableEmployeesUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;

    public DisableEmployeesUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards)
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
    }

    public async Task<Result<DisableEmployeesResponse>> HandleAsync(
        DisableEmployeesCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.EnsureCompanyExists(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<DisableEmployeesResponse>.FromError(companyRes);

        var memberGuardRes = await _memberGuards.ValidateMembersForDeactivation(cmd.CompanyId, cmd.EmployeeIds, ct);
        if (memberGuardRes.IsFailure)
            return Result<DisableEmployeesResponse>.FromError(memberGuardRes);

        foreach (var pos in memberGuardRes.Value!)
            pos.Deactivate();

        var disabledIds = memberGuardRes.Value!.Select(p => p.Id).ToArray();
        return Result<DisableEmployeesResponse>.Ok(new(disabledIds));
    }
}