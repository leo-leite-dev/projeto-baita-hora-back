using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Employees.Disable;

public sealed class DisableEmployeesUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;
    private readonly ICurrentCompany _currentCompany;

    public DisableEmployeesUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards,
        ICurrentCompany currentCompany)
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
        _currentCompany = currentCompany;
    }

    public async Task<Result<DisableEmployeesResponse>> HandleAsync(
        DisableMembersCommand cmd, CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var companyRes = await _companyGuards.EnsureCompanyExists(companyId, ct);
        if (companyRes.IsFailure)
            return Result<DisableEmployeesResponse>.FromError(companyRes);

        var memberGuardRes =
            await _memberGuards.ValidateMembersForDesactivation(companyId, cmd.EmployeeIds, ct);
        if (memberGuardRes.IsFailure)
            return Result<DisableEmployeesResponse>.FromError(memberGuardRes);

        foreach (var member in memberGuardRes.Value!)
            member.Desactivate();

        var disabledIds = memberGuardRes.Value!.Select(m => m.Id).ToArray();
        return Result<DisableEmployeesResponse>.Ok(new(disabledIds));
    }
}