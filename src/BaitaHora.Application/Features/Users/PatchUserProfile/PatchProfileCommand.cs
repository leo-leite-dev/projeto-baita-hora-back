using BaitaHora.Application.Features.Addresses.PatchAddress;

namespace BaitaHora.Application.Features.Users.PatchUserProfile;

public sealed record PatchUserProfileCommand
{
    public string? NewFullName { get; init; }
    public DateOnly? NewBirthDate { get; init; }
    public string? NewUserPhone { get; init; }
    public string? NewCpf { get; init; }
    public string? NewRg { get; init; }
    public PatchAddressCommand? Address { get; init; }
}