using CRole = BaitaHora.Contracts.Enums.CompanyRole;
using DRole = BaitaHora.Domain.Features.Companies.Enums.CompanyRole;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyRoleMapper
{
    public static DRole ToDomain(this CRole role) => role switch
    {
        CRole.Owner => DRole.Owner,
        CRole.Manager => DRole.Manager,
        CRole.Staff => DRole.Staff,
        CRole.Viewer => DRole.Viewer,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Valor de role inválido no contrato.")
    };

    public static CRole ToContract(this DRole role) => role switch
    {
        DRole.Owner => CRole.Owner,
        DRole.Manager => CRole.Manager,
        DRole.Staff => CRole.Staff,
        DRole.Viewer => CRole.Viewer,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Valor de role inválido no domínio.")
    };
}