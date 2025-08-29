using BaitaHora.Api.Mappers.Address;
using BaitaHora.Application.Features.Users.CreateUserProfile;
using BaitaHora.Application.Features.Users.PatchUserProfile;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Api.Mappers.Users;

public static class UserProfileApiMappers
{
    public static CreateUserProfileCommand ToUserProfileCommand(this CreateUserProfileRequest p)
        => new(p.FullName, p.Cpf, p.Rg, p.UserPhone, p.BirthDate, p.Address.ToAddressCommand());

    public static PatchUserProfileCommand ToUserProfileCommand(this PatchUserProfileRequest p)
        => new PatchUserProfileCommand
        {
            NewFullName = p.FullName,
            NewCpf = p.Cpf,
            NewRg = p.Rg,
            NewUserPhone = p.UserPhone,
            NewBirthDate = p.BirthDate,
            Address = p.Address?.ToAddressCommand()
        };
}