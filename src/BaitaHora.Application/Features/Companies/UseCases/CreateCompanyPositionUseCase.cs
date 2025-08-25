using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.Commands;

namespace BaitaHora.Application.Features.Companies.UseCase;

public sealed class CreateCompanyPositionUseCase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyPositionRepository _companyPositionRepository;

    public CreateCompanyPositionUseCase(
        ICompanyRepository companyRepository,
        ICompanyPositionRepository companyPositionRepository)
    {
        _companyRepository = companyRepository;
        _companyPositionRepository = companyPositionRepository;
    }

    public async Task<Result<CreateCompanyPositionResponse>> HandleAsync(CreateCompanyPositionCommand  cmd, CancellationToken ct)
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