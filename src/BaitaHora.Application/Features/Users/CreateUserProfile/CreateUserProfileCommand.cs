using BaitaHora.Application.Features.Addresses.Create;

namespace BaitaHora.Application.Features.Users.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    string FullName,
    DateTime? BirthDate,
    string UserPhone,
    string Cpf,
    string? Rg,
    CreateAddressCommand Address
);