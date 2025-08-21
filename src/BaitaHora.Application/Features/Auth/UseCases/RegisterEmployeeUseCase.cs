// using BaitaHora.Application.Common;
// using BaitaHora.Application.IRepositories;
// using BaitaHora.Application.IServices.Auth;
// using BaitaHora.Domain.Users.Entities;
// using BaitaHora.Domain.Users.ValueObjects;
// using Microsoft.Extensions.Logging;

// namespace BaitaHora.Application.Auth.UseCases.RegisterEmployee;

// public sealed class RegisterEmployeeUseCase
// {
//     private readonly IUserRepository _userRepository;
//     private readonly ICompanyRepository _companies;
//     private readonly ICompanyPermissionService _permissions;
//     private readonly ICompanyPositionRepository _positions;
//     private readonly IPasswordService _passwords;
//     private readonly IEmailService _emails;
//     private readonly IUnitOfWork _uow;
//     private readonly ILogger<RegisterEmployeeUseCase> _log;

//     public RegisterEmployeeUseCase(
//         IUserRepository userRepository,
//         ICompanyRepository companies,
//         ICompanyPermissionService permissions,
//         ICompanyPositionRepository positions,
//         IPasswordService passwords,
//         IEmailService emails,
//         IUnitOfWork uow,
//         ILogger<RegisterEmployeeUseCase> log)
//     {
//         _userRepository = userRepository;
//         _companies = companies;
//         _permissions = permissions;
//         _positions = positions;
//         _passwords = passwords;
//         _emails = emails;
//         _uow = uow;
//         _log = log;
//     }

//     public async Task<Result<Guid>> HandleAsync(RegisterEmployeeInput input, CancellationToken ct)
//     {
//         // 0) autorização de quem está criando
//         var auth = await _permissions.DemandAsync(input.CompanyId, input.ActorUserId, CompanyPermission.AddMember, ct);
//         if (auth.IsFailure) return Result<Guid>.From(auth, default!);

//         // 1) checagens base
//         var company = await _companies.GetByIdAsync(input.CompanyId, ct);
//         if (company is null)
//             return Result<Guid>.NotFound("Empresa não encontrada.", ResultCodes.NotFound.Company);

//         var position = await _positions.GetByIdAsync(input.PositionId, ct);
//         if (position is null || position.CompanyId != input.CompanyId)
//             return Result<Guid>.BadRequest("Cargo inválido para a empresa.", ResultCodes.BadRequest.InvalidReference);

//         // 2) VOs e invariantes
//         var email = Email.Parse(input.Email);
//         var username = Username.TryParseOrNull(input.Username);
//         var hash = _passwords.Hash(input.RawPassword);

//         var cpf = string.IsNullOrWhiteSpace(input.Profile.Cpf) ? null : CPF.Parse(input.Profile.Cpf);
//         var rg  = string.IsNullOrWhiteSpace(input.Profile.Rg)  ? null : RG.Parse(input.Profile.Rg);

//         // 3) unicidades
//         if (await _userRepository.ExistsByEmailAsync(email, ct))
//             return Result<Guid>.Conflict("E-mail já em uso.", ResultCodes.Conflict.User);

//         if (username is not null && await _users.ExistsByUsernameAsync(username, ct))
//             return Result<Guid>.Conflict("Username já em uso.", ResultCodes.Conflict.User);

//         var employee = User.CreateEmployee(
//             email, username, hash,
//             profile: UserProfile.FromInput(input.Profile, cpf, rg),
//             role: input.Role);

//         employee.AssignToCompany(position.CompanyId, position.Id);

//         await _userRepository.AddAsync(employee, ct);
//         await _uow.SaveChangesAsync(ct);

//         try
//         {
//             await _emails.SendWelcomeEmployeeAsync(email.Value, employee.Username?.Value ?? employee.Id.ToString(), company.Name, ct);
//         }
//         catch (Exception ex)
//         {
//             _log.LogWarning(ex, "Falha ao enviar e-mail de boas-vindas ao colaborador. userId={UserId}", employee.Id);
//         }

//         return Result<Guid>.Created(employee.Id, title: "Colaborador registrado.");
//     }
// }
