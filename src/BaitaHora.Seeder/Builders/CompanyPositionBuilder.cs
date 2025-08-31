using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.Enums;

namespace BaitaHora.Seeder.Builders;

public static class PositionBuilder
{
    public static CreatePositionRequest Build(string name, CompanyRole role)
        => new(name, role);

    public static CreatePositionRequest Default()
        => new("Cabeleireiro(a)", CompanyRole.Staff);
}