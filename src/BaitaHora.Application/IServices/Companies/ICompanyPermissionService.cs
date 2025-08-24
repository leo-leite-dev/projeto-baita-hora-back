using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.IServices.Auth;

public interface ICompanyPermissionService
{
    // Mantém compatibilidade
    Task<bool> CanAsync(Guid companyId, Guid userId, CompanyPermission required, CancellationToken ct);

    // Novo: verifica múltiplas permissões em OR (ANY)
    Task<bool> CanAnyAsync(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct);

    // Novo: obtém a máscara do usuário para a empresa (c/ cache por request)
    Task<CompanyPermission?> GetMaskAsync(Guid companyId, Guid userId, CancellationToken ct);

    // Helpers (bitwise) para uso interno/externo
    bool Has(CompanyPermission mask, CompanyPermission required);          // ALL por flag
    bool HasAny(CompanyPermission mask, IEnumerable<CompanyPermission> required);
}
