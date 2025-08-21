using BaitaHora.Application.Addresses.DTOs.Inputs;

namespace BaitaHora.Application.Users.DTOs;
public sealed record UserProfilePatch(
    string? FullName,
    DateTime? BirthDate,
    string? Phone,
    AddressPatch? Address,
    string? Cpf,
    string? Rg,
    string? ProfileImageUrl
);