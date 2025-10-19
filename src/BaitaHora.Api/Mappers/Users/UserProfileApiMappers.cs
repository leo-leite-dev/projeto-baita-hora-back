using BaitaHora.Api.Mappers.Address;
using BaitaHora.Application.Features.Users.CreateUserProfile;
using BaitaHora.Application.Features.Users.PatchUserProfile;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Api.Mappers.Users;

public static class UserProfileApiMappers
{
    public static CreateUserProfileCommand ToUserProfileCommand(this CreateUserProfileRequest p)
    => new CreateUserProfileCommand
    {
        FullName = p.FullName,
        Cpf = p.Cpf,
        Rg = p.Rg,
        Phone = p.Phone,
        BirthDate = p.BirthDate,
        Address = p.Address.ToAddressCommand()
    };

    public static PatchUserProfileCommand ToUserProfileCommand(this PatchUserProfileRequest p)
        => new PatchUserProfileCommand
        {
            NewFullName = p.FullName,
            NewCpf = p.Cpf,
            NewRg = p.Rg,
            NewUserPhone = p.Phone,
            NewBirthDate = p.BirthDate,
            Address = p.Address?.ToAddressCommand()
        };
}