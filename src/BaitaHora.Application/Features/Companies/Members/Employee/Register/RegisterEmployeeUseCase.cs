using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Users.Common;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Users.Entities;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Register;

public sealed class RegisterEmployeeUseCase
{
    private readonly IPasswordService _passwordService;
    private readonly ICompanyGuards _companyGuards;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyMemberRepository _memberRepository;

    public RegisterEmployeeUseCase(
        IPasswordService passwordService,
        ICompanyGuards companyGuards,
        IUserRepository userRepository,
        ICompanyMemberRepository memberRepository)
    {
        _passwordService = passwordService;
        _companyGuards = companyGuards;
        _userRepository = userRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<RegisterEmployeeResponse>> HandleAsync(
        RegisterEmployeeCommand request, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndServiceOfferings(request.CompanyId, ct);
        if (!companyRes.IsSuccess)
            return companyRes.MapError<RegisterEmployeeResponse>();

        var company = companyRes.Value!;

        var position = company.Positions.FirstOrDefault(p => p.Id == request.PositionId);
        if (position is null)
            return Result<RegisterEmployeeResponse>.BadRequest("Cargo inválido para esta empresa.");

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

        var response = new RegisterEmployeeResponse(
            user.Id,
            employee.FullName,
            user.Username.Value,
            user.UserEmail.Value,
            position.Id,
            position.Name);

        await _userRepository.AddAsync(user, ct);
        await _memberRepository.AddAsync(member, ct);

        return Result<RegisterEmployeeResponse>.Created(response);
    }
}