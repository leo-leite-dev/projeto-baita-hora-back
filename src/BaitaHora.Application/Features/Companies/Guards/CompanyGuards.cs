using BaitaHora.Application.Common;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public interface ICompanyGuards
{
    Task<Result<Company>> GetWithMembersAndPositionsOrNotFoundAsync(Guid companyId, CancellationToken ct);
}

public sealed class CompanyGuards : ICompanyGuards
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyGuards(ICompanyRepository companyRepository) => _companyRepository = companyRepository;

    public async Task<Result<Company>> GetWithMembersAndPositionsOrNotFoundAsync(Guid companyId, CancellationToken ct)
    {
        var company = await _companyRepository.GetByIdWithMembersAndPositionsAsync(companyId, ct);
        return company is null
            ? Result<Company>.NotFound("Empresa não encontrada.")
            : Result<Company>.Ok(company);
    }
}