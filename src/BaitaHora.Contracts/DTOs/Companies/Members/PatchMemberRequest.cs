namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record PatchMemberRequest(
    string? Email,
    string? Cpf,
    string? Rg
);