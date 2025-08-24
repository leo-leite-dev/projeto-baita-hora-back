using BaitaHora.Contracts.DTOS.Adress;

namespace BaitaHora.Contracts.DTOs.Users;

public sealed record CreateUserProfileRequest(
    string FullName,
    string UserPhone,
    DateTime? BirthDate,
    string Cpf,
    string? Rg,
    CreateAddressRequest Address
);