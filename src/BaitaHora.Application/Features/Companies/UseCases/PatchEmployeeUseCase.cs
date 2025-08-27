using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.Features.Companies.UseCases;

public sealed class PatchEmployeeUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyMemberGuards _companyMemberGuards;
    private readonly ICompanyPositionGuards _companyPositionGuards;

    public PatchEmployeeUseCase(
        IUserRepository userRepository,
        ICompanyMemberRepository companyMemberRepository,
        ICompanyGuards companyGuards,
        ICompanyMemberGuards companyMemberGuards,
        ICompanyPositionGuards companyPositionGuards)
    {
        _userRepository = userRepository;
        _companyMemberRepository = companyMemberRepository;
        _companyGuards = companyGuards;
        _companyMemberGuards = companyMemberGuards;
        _companyPositionGuards = companyPositionGuards;
    }

    public async Task<Result<PatchEmployeeResponse>> HandleAsync(PatchEmployeeCommand request, CancellationToken ct)
    {
        var companyResult = await _companyGuards.GetWithMembersAndPositionsOrNotFoundAsync(request.CompanyId, ct);
        if (!companyResult.IsSuccess)
            return companyResult.MapError<PatchEmployeeResponse>();
        var company = companyResult.Value!;

        var memberResult = _companyMemberGuards.GetMemberOrNotFound(company, request.EmployeeId, requireActive: false);
        if (!memberResult.IsSuccess)
            return memberResult.MapError<PatchEmployeeResponse>();
        var member = memberResult.Value!;

        var changed = false;
        var requiresSessionRefresh = false;

        if (request.PositionId is Guid posId)
        {
            var positionResult = _companyPositionGuards.GetValidPositionOrBadRequest(company, posId);
            if (!positionResult.IsSuccess)
                return positionResult.MapError<PatchEmployeeResponse>();

            var position = positionResult.Value!;

            if (member.SetPrimaryPosition(position))
                changed = true;

            var (roleChanged, mustRefresh) = member.SetRole(
                newRole: position.AccessLevel,
                allowOwnerLevel: false
            );

            if (roleChanged)
                changed = true;
            if (mustRefresh)
                requiresSessionRefresh = true;
        }

        User? user = null;
        if (request.Employee is not null)
        {
            user = await _userRepository.GetByIdAsync(member.UserId, ct);
            if (user is null)
                return Result<PatchEmployeeResponse>.NotFound("Membro da companhia não encontrado.");

            var changedUser = false;

            if (!string.IsNullOrWhiteSpace(request.Employee.UserEmail))
                changedUser |= user.SetEmail(Email.Parse(request.Employee.UserEmail));

            if (!string.IsNullOrWhiteSpace(request.Employee.Username))
                changedUser |= user.SetUsername(Username.Parse(request.Employee.Username));

            user = await _userRepository.GetByIdWithProfileAsync(member.UserId, ct);
            if (user is null)
                return Result<PatchEmployeeResponse>.NotFound("Membro da empresa não encontrado.");

            if (request.Employee.Profile is not null)
            {
                var p = request.Employee.Profile;

                if (!string.IsNullOrWhiteSpace(p.FullName))
                    changedUser |= user.Profile.SetFullName(p.FullName);

                if (p.BirthDate is { } birthDate)
                    changedUser |= user.Profile.SetBirthDate(birthDate);

                if (!string.IsNullOrWhiteSpace(p.UserPhone))
                    changedUser |= user.Profile.SetPhone(Phone.Parse(p.UserPhone));

                if (!string.IsNullOrWhiteSpace(p.Cpf))
                    changedUser |= user.Profile.SetCpf(CPF.Parse(p.Cpf));

                if (!string.IsNullOrWhiteSpace(p.Rg))
                    changedUser |= user.Profile.SetRg(RG.Parse(p.Rg));

                if (p.Address is not null)
                {
                    var addr = Address.Create(
                        street: p.Address.Street,
                        number: p.Address.Number,
                        complement: p.Address.Complement,
                        neighborhood: p.Address.Neighborhood,
                        city: p.Address.City,
                        state: p.Address.State,
                        zipCode: p.Address.ZipCode
                    );
                    changedUser |= user.Profile.SetAddress(addr);
                }
            }

            if (changedUser)
            {
                changed = true;
                await _userRepository.UpdateAsync(user, ct);
            }
        }

        if (!changed)
        {
            var ok = new PatchEmployeeResponse(
                EmployeeId: member.UserId,
                EmployeeName: member.User.Profile.FullName
            );
            return Result<PatchEmployeeResponse>.Ok(ok);
        }

        await _companyMemberRepository.UpdateAsync(member, ct);

        if (requiresSessionRefresh)
        {
            if (user is null)
            {
                user = await _userRepository.GetByIdAsync(member.UserId, ct);
                if (user is null)
                    return Result<PatchEmployeeResponse>.NotFound("Membro da companhia não encontrado.");
            }

            if (user.IncrementTokenVersion())
                await _userRepository.UpdateAsync(user, ct);
        }

        var response = new PatchEmployeeResponse(
            EmployeeId: member.UserId,
            EmployeeName: member.User.Profile.FullName
        );
        return Result<PatchEmployeeResponse>.Ok(response);
    }
}