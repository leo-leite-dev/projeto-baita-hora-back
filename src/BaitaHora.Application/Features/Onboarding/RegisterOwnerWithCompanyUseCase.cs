using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.Features.Users.CreateUser;

namespace BaitaHora.Application.Features.Onboarding;

public sealed class RegisterOwnerWithCompanyUseCase

{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordService _passwordService;

    public RegisterOwnerWithCompanyUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _passwordService = passwordService;
    }

    public async Task<Result<RegisterOwnerWithCompanyResponse>> HandleAsync(
        RegisterOwnerWithCompanyCommand request, CancellationToken ct)
    {
        var (owner, company) = Assembler.From(request);

        var profile = UserProfile.Create(owner.FullName, owner.Cpf, owner.UserPhone, owner.Address);

        if (owner.Rg is not null)
            profile.SetRg(owner.Rg.Value);

        if (owner.BirthDate is not null)
            profile.SetBirthDate(owner.BirthDate.Value);

        var user = User.Create(
            owner.UserEmail,
            owner.Username,
            owner.RawPassword,
            profile,
            _passwordService.Hash);

        var comp = Company.Create(
            company.CompanyName,
            company.Cnpj,
            company.Address,
            company.TradeName,
            company.CompanyPhone,
            company.CompanyEmail
        );

        user.SetRole(CompanyRole.Owner);
        comp.AddOwnerFounder(user.Id);

        await _userRepository.AddAsync(user, ct);
        await _companyRepository.AddAsync(comp, ct);

        var response = new RegisterOwnerWithCompanyResponse(user.Id, comp.Id, profile.FullName, comp.CompanyName.Value);
        return Result<RegisterOwnerWithCompanyResponse>.Created(response);
    }

    private static class Assembler
    {
        public static (OwnerVO owner, CompanyVO company) From(RegisterOwnerWithCompanyCommand cmd)
            => (BuildOwner(cmd.Owner), BuildCompany(cmd.Company));

        private static OwnerVO BuildOwner(CreateUserCommand u)
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

            return new OwnerVO(email, username, u.RawPassword, u.Profile.FullName, cpf, rg, phone, birth, addr);
        }

        private static CompanyVO BuildCompany(CreateCompanyWithOwnerCommand c)
        {
            var name = CompanyName.Parse(c.CompanyName);
            var cnpj = CNPJ.Parse(c.Cnpj);
            var email = Email.Parse(c.CompanyEmail);
            var phone = Phone.Parse(c.CompanyPhone);

            var addr = Address.Parse(
                street: c.Address.Street,
                number: c.Address.Number,
                neighborhood: c.Address.Neighborhood,
                city: c.Address.City,
                state: c.Address.State,
                zipCode: c.Address.ZipCode,
                complement: c.Address.Complement
            );

            return new CompanyVO(name, c.TradeName, cnpj, email, phone, addr);
        }
    }
}

public readonly record struct OwnerVO(
    Email UserEmail, Username Username, string RawPassword, string FullName,
    CPF Cpf, RG? Rg, Phone UserPhone, DateOnly? BirthDate, Address Address);

public readonly record struct CompanyVO(
    CompanyName CompanyName, string? TradeName, CNPJ Cnpj, Email CompanyEmail, Phone CompanyPhone, Address Address);