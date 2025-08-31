using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Positions.Enable;

public sealed class ActivatePositionsUseCase
{
    private readonly ICompanyGuards _companyGuards;

    public ActivatePositionsUseCase(ICompanyGuards companyGuards)
        => _companyGuards = companyGuards;

    //VERIFICAR PORQUE ACTIVE NAO ESTA PERSISTINDO
    public async Task<Result<ActivatePositionsResponse>> HandleAsync(
        ActivatePositionsCommand cmd, CancellationToken ct)
    {
        var companyRes = await _companyGuards.GetWithPositionsAndMembers(cmd.CompanyId, ct);
        if (companyRes.IsFailure)
            return Result<ActivatePositionsResponse>.FromError(companyRes);

        var company = companyRes.Value!;

        var ids = IdSet.Normalize(cmd.PositionIds);
        if (ids.Count == 0)
            return Result<ActivatePositionsResponse>.BadRequest("Nenhum serviço válido informado.");

        var notFound = IdSet.MissingFrom(ids, company.ServiceOfferings, s => s.Id);
        if (notFound.Count > 0)
            return Result<ActivatePositionsResponse>.NotFound($"Serviços não encontrados: {string.Join(", ", notFound)}");

        foreach (var s in company.ServiceOfferings.Where(x => ids.Contains(x.Id)))
            s.Activate();

        return Result<ActivatePositionsResponse>.Ok(new(ids));
    }
}