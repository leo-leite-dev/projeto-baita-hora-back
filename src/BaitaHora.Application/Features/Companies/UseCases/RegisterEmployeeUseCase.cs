using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Ports;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Companies.UseCase;

public sealed class RegisterEmployeeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly IPasswordService _passwordService;
    private readonly ICompanyPermissionService _permissionService;
    private readonly IUserIdentityPort _identity;
    private readonly ILogger<RegisterEmployeeUseCase> _logger;

    public RegisterEmployeeUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        ICompanyMemberRepository companyMemberRepository,
        IPasswordService passwordService,
        ICompanyPermissionService permissionService,
        IUserIdentityPort identity,
        ILogger<RegisterEmployeeUseCase> logger)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _passwordService = passwordService;
        _permissionService = permissionService;
        _identity = identity;
        _logger = logger;
    }

    public async Task<Result<RegisterEmployeeResponse>> HandleAsync(RegisterEmployeeCommand request, CancellationToken ct)
    {
        var currentUserId = _identity.GetUserId();

        var company = await _companyRepository.GetByIdWithMembersAndPositionsAsync(request.CompanyId, ct);
        if (company is null)
            return Result<RegisterEmployeeResponse>.NotFound("Empresa não encontrada.");

        var isOwner = company.Members.Any(m => m.UserId == currentUserId && m.Role == CompanyRole.Owner);
        if (!isOwner)
            return Result<RegisterEmployeeResponse>.Forbidden("Apenas o Owner pode adicionar funcionários.");

        var position = company.Positions.SingleOrDefault(p => p.Id == request.PositionId);
        if (position is null)
            return Result<RegisterEmployeeResponse>.BadRequest("Cargo inválido para esta empresa.");

        var input = Assembler.From(request.Employee);

        var profile = UserProfile.Create(
            input.FullName,
            input.Cpf,
            input.UserPhone,
            input.Address);

        if (input.Rg is not null) profile.SetRg(input.Rg.Value);
        if (input.BirthDate is not null) profile.SetBirthDate(input.BirthDate.Value);

        var user = User.Create(
            input.UserEmail,
            input.Username,
            input.RawPassword,
            profile,
            _passwordService.Hash);

        user.SetRole(position.AccessLevel);

        var member = company.AddMemberFromPosition(user.Id, position);

        await _userRepository.AddAsync(user, ct);
        await _companyMemberRepository.AddAsync(member, ct);

        var response = new RegisterEmployeeResponse(
            user.Id,
            input.FullName,
            user.Username.Value,
            user.UserEmail.Value,
            position.Id,
            position.PositionName);

        return Result<RegisterEmployeeResponse>.Created(response);
    }

    private static class Assembler
    {
        public static EmployeeVO From(UserCommand u)
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
                Address: addr
            );
        }
    }
}

public readonly record struct EmployeeVO(
    Email UserEmail, Username Username, string RawPassword, string FullName, CPF Cpf, RG? Rg,
    Phone UserPhone, DateTime? BirthDate, Address Address
);