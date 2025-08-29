using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyGuards : ICompanyGuards
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyGuards(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

    public async Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct)
    {
        var company = await _companyRepository.GetDetailsByIdAsync(companyId, ct);
        return company is null
            ? Result<Company>.NotFound("Empresa não encontrada.")
            : Result<Company>.Ok(company);
    }
}