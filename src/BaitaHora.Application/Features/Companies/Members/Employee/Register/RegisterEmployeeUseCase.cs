using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Common;
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
        var companyResult = await _companyGuards.ExistsCompany(request.CompanyId, ct);
        if (!companyResult.IsSuccess)
            return companyResult.MapError<RegisterEmployeeResponse>();

        var company = companyResult.Value!;

        var positionResult = _companyPositionGuards.GetValidPositionOrBadRequest(company, request.PositionId);
        if (!positionResult.IsSuccess)
            return positionResult.MapError<RegisterEmployeeResponse>();

        var position = positionResult.Value!;

        var employee = UserAssembler.BuildOwnerVO(request.Employee);

        var profile = UserProfile.Create(
             employee.FullName,
             employee.Cpf,
             employee.Rg,
             employee.UserPhone,
             employee.BirthDate,
             employee.Address);

        var user = User.Create(
            employee.UserEmail,
            employee.Username,
            employee.RawPassword,
            profile,
            _passwordService.Hash);

        var member = company.AddMemberWithPrimaryPosition(user.Id, position);

        await _userRepository.AddAsync(user, ct);
        await _companyMemberRepository.AddAsync(member, ct);

        var response = new RegisterEmployeeResponse(
            user.Id,
            employee.FullName,
            user.Username.Value,
            user.UserEmail.Value,
            position.Id,
            position.PositionName);

        return Result<RegisterEmployeeResponse>.Created(response);
    }
}