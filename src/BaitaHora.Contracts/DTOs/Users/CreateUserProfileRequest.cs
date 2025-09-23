using BaitaHora.Contracts.DTOS.Adresses;

namespace BaitaHora.Contracts.DTOs.Users;

public sealed record CreateUserProfileRequest(
    string FullName,
    string Cpf,
    string? Rg,
    string UserPhone,
    DateOnly? BirthDate,
    CreateAddressRequest Address
);