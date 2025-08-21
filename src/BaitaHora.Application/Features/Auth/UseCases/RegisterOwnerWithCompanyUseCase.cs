using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Companies.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Application.Features.Users.DTOs;
using BaitaHora.Application.Features.Companies.Inputs;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Application.IServices.Common;

namespace BaitaHora.Application.Features.Auth.UseCases;

public sealed class RegisterOwnerWithCompanyUseCase

{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUserUniquenessChecker _userUniquenessesChecker;
    private readonly ILogger<RegisterOwnerWithCompanyUseCase> _logger;

    public RegisterOwnerWithCompanyUseCase(
        IUserRepository userRepository,
        ICompanyRepository companyRepository,
        IPasswordService passwordService,
        IUserUniquenessChecker userUniquenessesChecker,
        ILogger<RegisterOwnerWithCompanyUseCase> logger)
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _passwordService = passwordService;
        _userUniquenessesChecker = userUniquenessesChecker;
        _logger = logger;
    }

    public async Task<Result<RegisterOwnerWithCompanyResponse>> HandleAsync(
        RegisterOwnerWithCompanyCommand request, CancellationToken ct)
    {
        var (owner, company) = Assembler.From(request);

        var uniqueness = await _userUniquenessesChecker.CheckAsync(
            owner.UserEmail, owner.Username, owner.Cpf, owner.Rg, excludingUserId: null, ct);

        if (!uniqueness.IsOk)
            return Result<RegisterOwnerWithCompanyResponse>.Conflict(
                string.Join(" ", uniqueness.Violations.Select(v => $"{v.Field}: {v.Message}"))
            );

        if (await _companyRepository.IsCompanyEmailTakenAsync(company.CompanyEmail, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Email de empresa já cadastrado.",
            ResultCodes.Conflict.UniqueViolation);

        if (await _companyRepository.IsCompanyNameTakenAsync(company.CompanyName, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Razão social já cadastrada.",
             ResultCodes.Conflict.UniqueViolation);

        if (await _companyRepository.IsCnpjTakenAsync(company.Cnpj, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("CNPJ já cadastrado.",
             ResultCodes.Conflict.UniqueViolation);

        try
        {
            var profile = UserProfile.Create(owner.FullName, owner.Cpf, owner.UserPhone, owner.Address);
            if (owner.Rg is not null) profile.SetRg(owner.Rg.Value);
            if (owner.BirthDate is not null) profile.SetBirthDate(owner.BirthDate.Value);

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

            var response = new RegisterOwnerWithCompanyResponse(user.Id, comp.Id);
            return Result<RegisterOwnerWithCompanyResponse>.Created(response);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex,
                "Violação de integridade ao registrar dono/empresa. Email={Email}, Username={Username}, CNPJ={Cnpj}",
                owner.UserEmail, owner.Username, company.Cnpj);

            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Violação de integridade (índice único).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro inesperado ao registrar dono/empresa. Email={Email}, Username={Username}, CNPJ={Cnpj}",
                owner.UserEmail, owner.Username, company.Cnpj);

            return Result<RegisterOwnerWithCompanyResponse>.ServerError(
                "Erro interno ao processar o cadastro do dono/empresa.");
        }
    }

    private static class Assembler
    {
        public static (OwnerVO owner, CompanyVO company) From(RegisterOwnerWithCompanyCommand input)
            => (BuildOwner(input.User), BuildCompany(input.Company));

        private static OwnerVO BuildOwner(UserInput u)
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

            return new OwnerVO(email, username, u.RawPassword, u.Profile.FullName, cpf, rg, phone, u.Profile.BirthDate, addr);
        }

        private static CompanyVO BuildCompany(CompanyInput c)
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
    CPF Cpf, RG? Rg, Phone UserPhone, DateTime? BirthDate, Address Address);

public readonly record struct CompanyVO(
    CompanyName CompanyName, string? TradeName, CNPJ Cnpj, Email CompanyEmail, Phone CompanyPhone, Address Address);