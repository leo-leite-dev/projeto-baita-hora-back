// using BaitaHora.Application.Common;
// using BaitaHora.Application.IRepositories;
// using BaitaHora.Application.IRepositories.Users;
// using BaitaHora.Application.IServices.Auth;
// using BaitaHora.Application.Users.DTOs;
// using BaitaHora.Domain.Commons.ValueObjects;
// using BaitaHora.Domain.Users.Entities;
// using BaitaHora.Domain.Users.ValueObjects;

// namespace BaitaHora.Application.Users.UseUseCase;

// public sealed class CreateUserUseCase
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IPasswordService _passwordService;

//     public CreateUserUseCase(IUserRepository userRepository, IPasswordService passwordService)
//     {
//         _userRepository = userRepository;
//         _passwordService = passwordService;
//     }

//     public async Task<Result<User>> HandleAsync(UserInput input, CancellationToken ct)
//     {
//         var errors = new List<string>();

//         if (!Email.TryParse(input.Email, out var email))
//             return Result<User>.Invalid("E-mail inválido.");

//         if (!Username.TryParse(input.Username, out var username))
//             return Result<User>.Invalid("Username inválido.");

//         if (!CPF.TryParse(input.Profile.Cpf, out var cpf))
//             return Result<User>.Invalid("CPF inválido.");

//         if (!Phone.TryParse(input.Profile.Phone, out var phone))
//             return Result<User>.Invalid("CPF inválido.");

//         RG? rg = null;
//         if (!string.IsNullOrWhiteSpace(input.Profile.Rg))
//         {
//             if (!RG.TryParse(input.Profile.Rg, out var rgValue))
//                 return Result<User>.Invalid("RG inválido.");
//             rg = rgValue;
//         }

//         if (!Address.TryParse(
//                 street: input.Profile.Address.Street,
//                 number: input.Profile.Address.Number,
//                 neighborhood: input.Profile.Address.Neighborhood,
//                 city: input.Profile.Address.City,
//                 state: input.Profile.Address.State,
//                 zipCode: input.Profile.Address.ZipCode,
//                 complement: input.Profile.Address.Complement,
//                 out var address,
//                 out var addressErrors))
//         {
//             return Result<User>.BadRequest(string.Join("; ", addressErrors.Select(e => e.Message)));
//         }

//         if (errors.Count > 0)
//             return Result<User>.Invalid(string.Join("; ", errors));

//         if (await _userRepository.IsUserEmailTakenAsync(email!.Value, null, ct))
//             return Result<User>.Conflict("E-mail já em uso.");

//         if (await _userRepository.IsUsernameTakenAsync(username!.Value, null, ct))
//             return Result<User>.Conflict("Username já em uso.");

//         var hash = _passwordService.Hash(input.RawPassword);

//         var profile = UserProfile.Create(
//             fullName: input.Profile.FullName,
//             cpf: cpf,
//             phone: phone,
//             address: address!);


//         if (rg is not null)
//             profile.SetRg(rg);

//         if (input.Profile.BirthDate.HasValue)
//             profile.SetBirthDate(input.Profile.BirthDate);

//         var user = User.Create(email, username, input.RawPassword, profile, _passwordService.Hash);

//         await _userRepository.AddAsync(user, ct);
//         return Result<User>.Created(user);
//     }
// }