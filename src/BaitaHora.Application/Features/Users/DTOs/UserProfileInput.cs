using BaitaHora.Application.Features.Commons.Inputs;

namespace BaitaHora.Application.Features.Users.DTOs;

public sealed record UserProfileInput(
    string FullName,
    DateTime? BirthDate,
    string UserPhone,
    AddressInput Address,
    string Cpf,
    string? Rg,
    string? ProfileImageUrl
);
