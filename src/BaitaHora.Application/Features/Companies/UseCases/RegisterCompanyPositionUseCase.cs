using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Companies.Enums;
using Microsoft.Extensions.Logging;
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

    public async Task<Result<CompanyPositionResponse>> HandleAsync(RegisterCompanyPositionCommand cmd, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdWithMembersAndPositionsAsync(cmd.CompanyId, ct);
        if (company is null)
            return Result<CompanyPositionResponse>.NotFound("Empresa não encontrada.");

        var isOwner = company.Members.Any(m => m.Role == CompanyRole.Owner);
        if (!isOwner)
            return Result<CompanyPositionResponse>.Forbidden("Apenas Fundador pode adicionar cargos.");

        var position = company.CreatePosition(cmd.PositionName, cmd.AccessLevel);

        await _companyPositionRepository.AddAsync(position, ct);

        var response = new CompanyPositionResponse(company.Id, position.PositionName);
        return Result<CompanyPositionResponse>.Created(response);
    }
}