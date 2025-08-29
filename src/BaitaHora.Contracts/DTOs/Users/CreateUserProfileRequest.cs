using BaitaHora.Contracts.DTOS.Adress;

namespace BaitaHora.Contracts.DTOs.Users;

public sealed record CreateUserProfileRequest(
    string FullName,
    string Cpf,
    string? Rg,
    string UserPhone,
    DateOnly? BirthDate,
    CreateAddressRequest Address
);