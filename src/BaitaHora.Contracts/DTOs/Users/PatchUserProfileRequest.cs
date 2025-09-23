using BaitaHora.Contracts.DTOS.Adresses;

namespace BaitaHora.Contracts.DTOs.Users;

public sealed record PatchUserProfileRequest(
    string? FullName,
    DateOnly? BirthDate,
    string? UserPhone,
    string? Cpf,
    string? Rg,
    PatchAddressRequest? Address
);