using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreateCompanyPositionUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public CreateCompanyPositionUseCase(ICompanyRepository companyRepository)
        => _companyRepository = companyRepository;

    public async Task<Result<CreateCompanyPositionResponse>> HandleAsync(
        CreateCompanyPositionCommand cmd, CancellationToken ct)
    {
        var company = await _companyRepository.GetDetailsByIdAsync(cmd.CompanyId, ct);
        if (company is null)
            return Result<CreateCompanyPositionResponse>.NotFound("Empresa não encontrada.");

        var position = company.AddPosition(cmd.PositionName, cmd.AccessLevel);

        var response = new CreateCompanyPositionResponse(
            position.Id,
            company.Id,
            position.PositionName,
            position.AccessLevel.ToString()
        );

        return Result<CreateCompanyPositionResponse>.Created(response);
    }
}