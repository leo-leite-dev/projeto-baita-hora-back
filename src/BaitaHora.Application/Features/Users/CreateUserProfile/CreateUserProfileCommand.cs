using BaitaHora.Application.Features.Addresses.Create;

namespace BaitaHora.Application.Features.Users.CreateUserProfile;

public sealed record CreateUserProfileCommand
{
    public required string FullName { get; init; }
    public required string Cpf { get; init; }
    public string? Rg { get; init; }
    public required string Phone { get; init; }
    public DateOnly? BirthDate { get; init; }
    public required CreateAddressCommand Address { get; init; }
}