using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;

namespace BaitaHora.Application.Features.Companies.UseCase;

public sealed class RegisterCompanyPositionUseCase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyPositionRepository _companyPositionRepository;

    public RegisterCompanyPositionUseCase(
        ICompanyRepository companyRepository,
        ICompanyPositionRepository companyPositionRepository)
    {
        _companyRepository = companyRepository;
        _companyPositionRepository = companyPositionRepository;
    }

    public async Task<Result<CreateCompanyPositionResponse>> HandleAsync(RegisterCompanyPositionCommand cmd, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdWithMembersAndPositionsAsync(cmd.CompanyId, ct);
        if (company is null)
            return Result<CreateCompanyPositionResponse>.NotFound("Empresa não encontrada.");

        var isOwner = company.Members.Any(m => m.Role == CompanyRole.Owner);
        if (!isOwner)
            return Result<CreateCompanyPositionResponse>.Forbidden("Apenas Fundador pode adicionar cargos.");

        var position = company.CreatePosition(cmd.PositionName, cmd.AccessLevel);

        await _companyPositionRepository.AddAsync(position, ct);

        var response = new CreateCompanyPositionResponse(company.Id, position.PositionName);
        return Result<CreateCompanyPositionResponse>.Created(response);
    }
}