using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.Enums;

namespace BaitaHora.Seeder.Builders;

public static class CompanyPositionBuilder
{
    public static CreateCompanyPositionRequest Build(string name, CompanyRole role)
        => new(name, role);

    public static CreateCompanyPositionRequest Default()
        => new("Cabeleireiro(a)", CompanyRole.Staff);
}