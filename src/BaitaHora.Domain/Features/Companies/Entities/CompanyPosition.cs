using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Domain.Features.Companies.Entities;

public class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = null!;
    public CompanyRole AccessLevel { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Company Company { get; private set; } = null!;

    private CompanyPosition() { }

    public CompanyPosition(Guid companyId, string name, CompanyRole accessLevel, bool allowOwnerLevel = false)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.");

        if (accessLevel == CompanyRole.Owner && !allowOwnerLevel)
            throw new InvalidOperationException("Cargo de nível Owner só pode ser criado no bootstrap da empresa (ex.: Fundador).");

        CompanyId = companyId;
        Name = name.Trim();
        AccessLevel = accessLevel;
        IsActive = true;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.");
        Name = name.Trim();
    }


    public void SetAccessLevel(CompanyRole level) => AccessLevel = level;
    public void Deactivate() => IsActive = false;
}