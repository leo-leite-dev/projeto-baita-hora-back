using BaitaHora.Application.Features.Commons.Commands;

namespace BaitaHora.Application.Features.Users.Commands;

public sealed record UserProfileCommand(
    string FullName,
    DateTime? BirthDate,
    string UserPhone,
    string Cpf,
    string? Rg,
    AddressCommand Address
);