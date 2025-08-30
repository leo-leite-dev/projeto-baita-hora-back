using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchCompanyPositionUseCase
{
    private readonly ICompanyGuards _companyGuards;
    private readonly ICompanyPositionRepository _companyPositionRepository;

    public PatchCompanyPositionUseCase(
        ICompanyGuards companyGuards,
        ICompanyPositionRepository companyPositionRepository)
    {
        _companyGuards = companyGuards;
        _companyPositionRepository = companyPositionRepository;
    }

    public async Task<Result<PatchCompanyPositionResponse>> HandleAsync(
        PatchCompanyPositionCommand request, CancellationToken ct)
    {
        var compRes = await _companyGuards.ExistsCompany(request.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<PatchCompanyPositionResponse>.FromError(compRes);

        var company = compRes.Value!;

        var position = await _companyPositionRepository.GetByIdAsync(request.PositionId, ct);
        if (position is null)
            return Result<PatchCompanyPositionResponse>.NotFound("Cargo não encontrado.");

        if (!string.IsNullOrWhiteSpace(request.NewPositionName))
            company.RenamePosition(request.PositionId, request.NewPositionName);

        if (request.NewAccessLevel.HasValue)
            position.ChangeAccessLevel(request.NewAccessLevel.Value);

        var response = new PatchCompanyPositionResponse(position.Id, position.Name);
        return Result<PatchCompanyPositionResponse>.Ok(response);
    }
}