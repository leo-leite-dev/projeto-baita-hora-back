using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;

namespace BaitaHora.Application.Features.Companies.Employees.Activate;

public sealed class ActivateEmployeesUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _memberGuards;
    private readonly ICurrentCompany _currentCompany; 

    public ActivateEmployeesUseCase(
        ICompanyGuards companyGuards,
        ICompanyMemberGuards memberGuards,
        ICurrentCompany currentCompany)             
    {
        _companyGuards = companyGuards;
        _memberGuards = memberGuards;
        _currentCompany = currentCompany;           
    }

    public async Task<Result<ActivateEmployeesResponse>> HandleAsync(
        ActivateMembersCommand cmd, CancellationToken ct)
    {
        var companyId = _currentCompany.Id;

        var companyRes = await _companyGuards.EnsureCompanyExists(companyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivateEmployeesResponse>.FromError(companyRes);

        var memberGuardRes = await _memberGuards.ValidateMembersForActivation(companyId, cmd.EmployeeIds, ct);
        if (memberGuardRes.IsFailure)
            return Result<ActivateEmployeesResponse>.FromError(memberGuardRes);

        foreach (var member in memberGuardRes.Value!)
            member.Activate();

        var activatedIds = memberGuardRes.Value!.Select(m => m.Id).ToArray();
        return Result<ActivateEmployeesResponse>.Ok(new(activatedIds));
    }
}