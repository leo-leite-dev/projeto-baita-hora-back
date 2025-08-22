using BaitaHora.Api.Contracts.Companies.Requests;

namespace BaitaHora.Api.Contracts.Users.Requests;

public sealed record UserProfileRequest(
    string FullName,
    string UserPhone,
    DateTime? BirthDate,
    string Cpf,
    string? Rg,
    AddressRequest Address
);