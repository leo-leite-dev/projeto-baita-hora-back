using BaitaHora.Application.Features.Addresses.Dtos;

namespace BaitaHora.Application.Features.Users.Dtos;

public sealed record UserProfileDto(
    string FullName,
    string Cpf,
    string? Rg,
    DateTime? BirthDate,
    string Phone,
    AddressDto Address,
    string? ProfileImageUrl
);