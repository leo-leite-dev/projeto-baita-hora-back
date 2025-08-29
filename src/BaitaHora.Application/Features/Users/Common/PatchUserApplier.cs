using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace BaitaHora.Application.Features.Users.Common;

public static class PatchUserApplier
{
    public static void Apply(User user, PatchUserCommand cmd)
    {
        if (!string.IsNullOrWhiteSpace(cmd.NewUserEmail))
            user.SetEmail(Email.Parse(cmd.NewUserEmail));

        if (!string.IsNullOrWhiteSpace(cmd.NewUsername))
            user.RenameUserName(Username.Parse(cmd.NewUsername));

        if (cmd.NewProfile is null)
            return;

        if (cmd.NewProfile is not null)
        {
            var p = cmd.NewProfile;

            if (!string.IsNullOrWhiteSpace(p.NewFullName))
                user.Profile.RenameFullName(p.NewFullName);

            if (p.NewBirthDate is DateOnly d)
            {
                var dob = DateOfBirth.Parse(d); 
                user.Profile.ChangeBirthDate(dob);
            }

            if (!string.IsNullOrWhiteSpace(p.NewUserPhone))
                user.Profile.ChangePhone(Phone.Parse(p.NewUserPhone));

            if (!string.IsNullOrWhiteSpace(p.NewCpf))
                user.Profile.ChangeCpf(CPF.Parse(p.NewCpf));

            if (!string.IsNullOrWhiteSpace(p.NewRg))
                user.Profile.ChangeRg(RG.Parse(p.NewRg));

            if (p.Address is not null)
            {
                var a = p.Address;

                var merged = Address.Parse(
                    street: a.NewStreet ?? user.Profile.Address.Street,
                    number: a.NewNumber ?? user.Profile.Address.Number,
                    neighborhood: a.NewNeighborhood ?? user.Profile.Address.Neighborhood,
                    city: a.NewCity ?? user.Profile.Address.City,
                    state: a.NewState ?? user.Profile.Address.State,
                    zipCode: a.NewZipCode ?? user.Profile.Address.ZipCode,
                    complement: a.NewComplement ?? user.Profile.Address.Complement
                );

                user.Profile.ChangeAddress(merged);
            }
        }
    }
}