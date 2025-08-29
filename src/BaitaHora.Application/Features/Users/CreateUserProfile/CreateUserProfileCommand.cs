using BaitaHora.Application.Features.Addresses.Create;

namespace BaitaHora.Application.Features.Users.CreateUserProfile;

public sealed record CreateUserProfileCommand(
    string FullName,
    string Cpf,
    string? Rg,
    string UserPhone,
    DateOnly? BirthDate,
    CreateAddressCommand Address
);