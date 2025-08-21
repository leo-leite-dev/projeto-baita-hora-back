using BaitaHora.Application.Features.Commons.Inputs;

namespace BaitaHora.Application.Features.Users.DTOs;
public sealed record UserProfilePatch(
    string? FullName,
    DateTime? BirthDate,
    string? Phone,
    AddressPatch? Address,
    string? Cpf,
    string? Rg,
    string? ProfileImageUrl
);