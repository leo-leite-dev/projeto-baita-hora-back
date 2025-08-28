using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Register;

public sealed class RegisterEmployeeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly IPasswordService _passwordService;
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;

    public RegisterEmployeeUseCase(
        IUserRepository userRepository,
        ICompanyMemberRepository companyMemberRepository,
        IPasswordService passwordService,
        ICompanyGuards companyGuards,
        ICompanyPositionGuards companyPositionGuards)
    {
        _userRepository = userRepository;
        _companyMemberRepository = companyMemberRepository;
        _passwordService = passwordService;
        _companyGuards = companyGuards;
        _companyPositionGuards = companyPositionGuards;
    }

    public async Task<Result<RegisterEmployeeResponse>> HandleAsync(
        RegisterEmployeeCommand request, CancellationToken ct)
    {
        var companyResult = await _companyGuards.GetWithMembersAndPositionsOrNotFoundAsync(request.CompanyId, ct);
        if (!companyResult.IsSuccess)
            return companyResult.MapError<RegisterEmployeeResponse>();

        var company = companyResult.Value!;

        var positionResult = _companyPositionGuards
            .GetValidPositionOrBadRequest(company, request.PositionId);
        if (!positionResult.IsSuccess)
            return positionResult.MapError<RegisterEmployeeResponse>();

        var position = positionResult.Value!;

        var input = Assembler.From(request.Employee);

        var profile = UserProfile.Create(input.FullName, input.Cpf, input.UserPhone, input.Address);

        if (input.Rg is not null)
            profile.SetRg(input.Rg.Value);

        if (input.BirthDate is not null)
            profile.SetBirthDate(input.BirthDate.Value);

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
        public static EmployeeVO From(CreateUserCommand u)
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

            DateOnly? birth = u.Profile.BirthDate is DateTime bdt
                ? DateOnly.FromDateTime(bdt)
                : (DateOnly?)null;

            return new EmployeeVO(
                UserEmail: email,
                Username: username,
                RawPassword: u.RawPassword,
                FullName: u.Profile.FullName,
                Cpf: cpf,
                Rg: rg,
                UserPhone: phone,
                BirthDate: birth,
                Address: addr
            );
        }
    }

    public readonly record struct EmployeeVO(
        Email UserEmail,
        Username Username,
        string RawPassword,
        string FullName,
        CPF Cpf,
        RG? Rg,
        Phone UserPhone,
        DateOnly? BirthDate,
        Address Address
    );
}