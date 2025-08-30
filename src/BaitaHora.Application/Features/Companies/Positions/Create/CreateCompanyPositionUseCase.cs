using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreateCompanyPositionUseCase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyPositionRepository _companyPositionRepository;
    private readonly ICompanyGuards _companyGuards;

    public CreateCompanyPositionUseCase(
        ICompanyGuards companyGuards,
        ICompanyRepository companyRepository,
        ICompanyPositionRepository companyPositionRepository)
    {
        _companyGuards = companyGuards;
        _companyRepository = companyRepository;
        _companyPositionRepository = companyPositionRepository;
    }

    public async Task<Result<CreateCompanyPositionResponse>> HandleAsync(
        CreateCompanyPositionCommand cmd, CancellationToken ct)
    {
        var compRes = await _companyGuards.ExistsCompany(cmd.CompanyId, ct);
        if (compRes.IsFailure)
            return Result<CreateCompanyPositionResponse>.FromError(compRes);

        var company = compRes.Value!;

        var position = company.AddPosition(cmd.PositionName, cmd.AccessLevel);

        await _companyPositionRepository.AddAsync(position);
        await _companyRepository.UpdateAsync(company);

        var response = new CreateCompanyPositionResponse(
            position.Id,
            company.Id,
            position.Name,
            position.AccessLevel.ToString()
        );

        return Result<CreateCompanyPositionResponse>.Created(response);
    }
}