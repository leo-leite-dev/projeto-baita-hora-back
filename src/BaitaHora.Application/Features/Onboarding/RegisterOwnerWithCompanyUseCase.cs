using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Application.Features.Companies.Common;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Companies.Entities;

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
        var owner = UserAssembler.BuildOwnerVO(request.Owner);

        var profile = UserProfile.Create(
            owner.FullName,
            owner.Cpf,
            owner.Rg,
            owner.UserPhone,
            owner.BirthDate,
            owner.Address
        );

        var user = User.Create(
            owner.UserEmail,
            owner.Username,
            owner.RawPassword,
            profile,
            _passwordService.Hash
        );

        var compVO = CompanyAssembler.BuildCompanyVO(request.Company);

        var company = Company.Create(
            companyName: compVO.CompanyName,
            cnpj: compVO.Cnpj,        
            address: compVO.Address,      
            tradeName: compVO.TradeName,     
            companyPhone: compVO.CompanyPhone,  
            companyEmail: compVO.CompanyEmail  
        );

        company.AddOwnerFounder(user.Id);

        await _userRepository.AddAsync(user, ct);
        await _companyRepository.AddAsync(company, ct);

        var response = new RegisterOwnerWithCompanyResponse(
            user.Id,
            company.Id,
            profile.Name,
            company.Name
        );

        return Result<RegisterOwnerWithCompanyResponse>.Created(response);
    }
}