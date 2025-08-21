using BaitaHora.Application.Auth.Commands;
using BaitaHora.Application.Auth.DTO.Responses;
using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Users.DTOs;
using BaitaHora.Application.Companies.Inputs;
using BaitaHora.Domain.Commons.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Users.Entities;
using BaitaHora.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class RegisterOwnerWithCompanyUseCase

{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<RegisterOwnerWithCompanyUseCase> _logger;

    public RegisterOwnerWithCompanyUseCase(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        ICompanyRepository companyRepository,
        IPasswordService passwordService,
        IUnitOfWork uow,
        ILogger<RegisterOwnerWithCompanyUseCase> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _companyRepository = companyRepository;
        _passwordService = passwordService;
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<RegisterOwnerWithCompanyResponse>> HandleAsync(
        RegisterOwnerWithCompanyCommand request, CancellationToken ct)
    {
        var (owner, company) = Assembler.From(request);

        if (await _userRepository.IsUserEmailTakenAsync(owner.Email, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("E-mail de usuário já cadastrado.");

        if (await _userRepository.IsUsernameTakenAsync(owner.Username, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Username já cadastrado.");

        if (await _userProfileRepository.IsCpfTakenAsync(owner.Cpf, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Cpf já cadastrado.");

        if (owner.Rg is not null && await _userProfileRepository.IsRgTakenAsync(owner.Rg.Value, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("RG já cadastrado.");

        if (await _companyRepository.IsCompanyEmailTakenAsync(company.Email, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Email de empresa já cadastrado.");

        if (await _companyRepository.IsCompanyNameTakenAsync(company.CompanyName, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Razão social já cadastrado.");

        if (await _companyRepository.IsCnpjTakenAsync(company.Cnpj, null, ct))
            return Result<RegisterOwnerWithCompanyResponse>.Conflict("CNPJ já cadastrado.");

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var profile = UserProfile.Create(owner.FullName, owner.Cpf, owner.Phone, owner.Address);
            if (owner.Rg is not null) profile.SetRg(owner.Rg.Value);
            if (owner.BirthDate is not null) profile.SetBirthDate(owner.BirthDate.Value);

            var user = User.Create(owner.Email, owner.Username, owner.RawPassword, profile, _passwordService.Hash);

            var comp = Company.Create(
                companyName: company.CompanyName,
                cnpj: company.Cnpj,
                email: company.Email,
                phone: company.Phone,
                address: company.Address,
                tradeName: company.TradeName
            );

            await _userRepository.AddAsync(user, ct);
            await _companyRepository.AddAsync(comp, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(tx, ct);

            var response = new RegisterOwnerWithCompanyResponse(user.Id, comp.Id);
            return Result<RegisterOwnerWithCompanyResponse>.Created(response);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex,
                "Persistência falhou ao registrar dono/empresa. Email={Email}, Username={Username}, CNPJ={Cnpj}",
                owner.Email, owner.Username, company.Cnpj);

            await _uow.RollbackTransactionAsync(tx, ct);

            return Result<RegisterOwnerWithCompanyResponse>.Conflict("Violação de integridade (índice único).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro inesperado ao registrar dono/empresa. Email={Email}, Username={Username}, CNPJ={Cnpj}",
                owner.Email, owner.Username, company.Cnpj);

            await _uow.RollbackTransactionAsync(tx, ct);

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
            var email = Email.Parse(u.Email);
            var username = Username.Parse(u.Username);
            var cpf = CPF.Parse(u.Profile.Cpf);
            var phone = Phone.Parse(u.Profile.Phone);

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
            var email = Email.Parse(c.Email);
            var phone = Phone.Parse(c.Phone);

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
    Email Email, Username Username, string RawPassword, string FullName,
    CPF Cpf, RG? Rg, Phone Phone, DateTime? BirthDate, Address Address);

public readonly record struct CompanyVO(
    CompanyName CompanyName, string? TradeName, CNPJ Cnpj, Email Email, Phone Phone, Address Address);