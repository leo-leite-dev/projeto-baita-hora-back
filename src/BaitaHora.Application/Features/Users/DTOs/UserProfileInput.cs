using BaitaHora.Application.Addresses.DTOs.Inputs;

namespace BaitaHora.Application.Users.DTOs;

public sealed record UserProfileInput(
    string FullName,
    DateTime? BirthDate,
    string Phone,
    AddressInput Address,
    string Cpf,
    string? Rg,
    string? ProfileImageUrl
);
