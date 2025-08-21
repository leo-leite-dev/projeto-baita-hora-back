using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Users.DTOs;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Common;
using BaitaHora.Application.Ports;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Domain.Permissions;
using Microsoft.Extensions.Logging;

public sealed class RegisterEmployeeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordService _passwordService;
    private readonly ICompanyPermissionService _permissionService;
    private readonly IUserUniquenessChecker _userUniquenessChecker;
    private readonly IUserIdentityPort _identity;
    private readonly ILogger<RegisterEmployeeUseCase> _logger;

    public RegisterEmployeeUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPasswordService passwordService,
        ICompanyPermissionService permissionService,
        IUserUniquenessChecker userUniquenessChecker,
        IUserIdentityPort identity,
        ILogger<RegisterEmployeeUseCase> logger)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _passwordService = passwordService;
        _permissionService = permissionService;
        _userUniquenessChecker = userUniquenessChecker;
        _identity = identity;
        _logger = logger;
    }

    public async Task<Result<RegisterEmployeeResponse>> HandleAsync(RegisterEmployeeCommand request, CancellationToken ct)
    {
        var currentUserId = _identity.GetUserId();

        // var canAdd = await _permissionService.CanAsync(request.CompanyId, currentUserId, CompanyPermission.AddMember, ct);
        // if (!canAdd)
        //     return Result<RegisterEmployeeResponse>.Forbidden("Sem permissão para adicionar membros.");

        var company = await _companyRepository.GetByIdWithMembersAndPositionsAsync(request.CompanyId, ct);
        if (company is null)
            return Result<RegisterEmployeeResponse>.NotFound("Empresa não encontrada.");

        var isOwner = company.Members.Any(m => m.UserId == currentUserId && m.Role == CompanyRole.Owner);
        if (!isOwner)
            return Result<RegisterEmployeeResponse>.Forbidden("Apenas o Owner pode adicionar funcionários.");

        var position = company.Positions.SingleOrDefault(p => p.Id == request.PositionId);
        if (position is null)
            return Result<RegisterEmployeeResponse>.BadRequest("Cargo inválido para esta empresa.");

        var employee = Assembler.From(request.User, request.Role);

        var uniqueness = await _userUniquenessChecker.CheckAsync(employee.UserEmail, employee.Username, employee.Cpf, employee.Rg, null, ct);
        if (!uniqueness.IsOk)
            return Result<RegisterEmployeeResponse>.Conflict(string.Join(" ", uniqueness.Violations.Select(v => $"{v.Field}: {v.Message}")));

        var profile = UserProfile.Create(
            employee.FullName,
            employee.Cpf,
            employee.UserPhone,
            employee.Address);

        if (employee.Rg is not null)
            profile.SetRg(employee.Rg.Value);

        if (employee.BirthDate is not null)
            profile.SetBirthDate(employee.BirthDate.Value);

        var user = User.Create(
            employee.UserEmail,
            employee.Username,
            employee.RawPassword,
            profile,
            _passwordService.Hash);

        company.AddMemberFromPosition(user.Id, position);

        await _userRepository.AddAsync(user, ct);
        await _companyRepository.UpdateAsync(company, ct);

        var response = new RegisterEmployeeResponse(user.Id);
        return Result<RegisterEmployeeResponse>.Created(response);
    }

    private static class Assembler
    {
        public static EmployeeVO From(UserInput u, CompanyRole? role)
        {
            var email = Email.Parse(u.UserEmail);
            var username = Username.Parse(u.Username);
            var cpf = CPF.Parse(u.Profile.Cpf);
            var phone = Phone.Parse(u.Profile.UserPhone);
            RG? rg = string.IsNullOrWhiteSpace(u.Profile.Rg) ? default : RG.Parse(u.Profile.Rg);

            var addr = Address.Parse(
                street: u.Profile.Address.Street,
                number: u.Profile.Address.Number,
                neighborhood: u.Profile.Address.Neighborhood,
                city: u.Profile.Address.City,
                state: u.Profile.Address.State,
                zipCode: u.Profile.Address.ZipCode,
                complement: u.Profile.Address.Complement
            );

            return new EmployeeVO(
                UserEmail: email,
                Username: username,
                RawPassword: u.RawPassword,
                FullName: u.Profile.FullName,
                Cpf: cpf,
                Rg: rg,
                UserPhone: phone,
                BirthDate: u.Profile.BirthDate,
                Address: addr,
                Role: role
            );
        }
    }
}

public readonly record struct EmployeeVO(
    Email UserEmail, Username Username, string RawPassword, string FullName, CPF Cpf, RG? Rg,
    Phone UserPhone, DateTime? BirthDate, Address Address, CompanyRole? Role
);