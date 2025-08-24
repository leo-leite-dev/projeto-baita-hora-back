using BaitaHora.Application.Features.Commons.Commands;

namespace BaitaHora.Application.Features.Users.Commands;

public sealed record CreateUserProfileCommand(
    string FullName,
    DateTime? BirthDate,
    string UserPhone,
    string Cpf,
    string? Rg,
    CreateAddressCommand Address
);