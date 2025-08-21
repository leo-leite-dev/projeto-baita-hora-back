using BaitaHora.Application.Features.Commons.Requests;

namespace BaitaHora.Application.Features.Users.DTOs;

public sealed record UserProfileRequest(
    string FullName,
    string Cpf,
    string? Rg,
    DateTime? BirthDate,
    string Phone,
    AddressRequest Address
);