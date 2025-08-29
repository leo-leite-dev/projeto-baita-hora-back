using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Contracts.DTOs.Companies.Members;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Api.Mappers.Users;

public static class UserApiMappers
{
    public static CreateUserCommand ToUserCommand(this RegisterEmployeeRequest r)
        => new(r.Employee.UserEmail, r.Employee.Username, r.Employee.RawPassword, r.Employee.Profile.ToUserProfileCommand());

    public static PatchUserCommand ToUserCommand(this PatchUserRequest r)
      => new PatchUserCommand
      {
          NewUserEmail = r.UserEmail,
          NewUsername = r.Username,
          NewProfile = r.Profile?.ToUserProfileCommand()
      };
}