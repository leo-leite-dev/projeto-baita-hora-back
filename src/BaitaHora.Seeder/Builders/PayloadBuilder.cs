using BaitaHora.Contracts.DTOS.Auth;        // ou DTOs.Companies, se for o teu caso
using BaitaHora.Contracts.DTOs.Users;
using BaitaHora.Contracts.DTOs.Companies;

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
}
