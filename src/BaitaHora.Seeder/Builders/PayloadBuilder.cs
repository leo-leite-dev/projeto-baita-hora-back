using BaitaHora.Contracts.DTOS.Auth;        
using BaitaHora.Contracts.DTOs.Users;
using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.Enums;

namespace BaitaHora.Seeder.Builders;

public static class PayloadBuilder
{
    public static RegisterOwnerWithCompanyRequest BuildRegisterOwnerWithCompany()
    {
        var company = CompanyBuilder.Default();
        var owner = OwnerBuilder.Default();
        return new RegisterOwnerWithCompanyRequest(Owner: owner, Company: company);
    }

    public static RegisterEmployeeRequest BuildRegisterEmployee(Guid positionId)
    {
        var employee = EmployeeBuilder.Default();
        return new RegisterEmployeeRequest(PositionId: positionId, Employee: employee);
    }

    public static RegisterEmployeeRequest BuildRegisterEmployee(Guid positionId, CreateUserRequest employee)
        => new(PositionId: positionId, Employee: employee);

    public static CreateCompanyPositionRequest BuildCreateCompanyPosition(string name, CompanyRole role)
        => new(name, role);

    public static CreateCompanyPositionRequest BuildCreateCompanyPosition(CompanyRole role)
    {
        var def = CompanyPositionBuilder.Default();
        return new CreateCompanyPositionRequest(def.PositionName, role);
    }
}
