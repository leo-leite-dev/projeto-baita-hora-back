using BaitaHora.Application.Addresses.DTOs.Requests;

namespace BaitaHora.Application.Users.DTOs;

public sealed record UserProfileRequest(
    string FullName,
    string Cpf,
    string? Rg,
    DateTime? BirthDate,
    string Phone,
    AddressRequest Address
);