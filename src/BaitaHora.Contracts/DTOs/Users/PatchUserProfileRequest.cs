using BaitaHora.Contracts.DTOS.Adresses;

namespace BaitaHora.Contracts.DTOs.Users;

public sealed record PatchUserProfileRequest(
    string? FullName,
    DateOnly? BirthDate,
    string? Phone,
    string? Cpf,
    string? Rg,
    PatchAddressRequest? Address
);