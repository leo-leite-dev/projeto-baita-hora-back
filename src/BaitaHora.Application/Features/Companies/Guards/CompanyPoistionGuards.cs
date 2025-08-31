using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class PositionGuards : IPositionGuards
{
    private readonly ICompanyPositionRepository _positionRepository;

    public PositionGuards(ICompanyPositionRepository positionRepository)
        => _positionRepository = positionRepository;

    public Result<CompanyPosition> GetValidPositionOrBadRequest(Company company, Guid positionId)
    {
        var position = company.Positions.SingleOrDefault(p => p.Id == positionId);
        if (position is null)
            return Result<CompanyPosition>.BadRequest("Cargo inválido para esta empresa.");

        if (!position.IsActive)
            return Result<CompanyPosition>.BadRequest("Não é possível atribuir um cargo inativo.");

        if (position.CompanyId != company.Id)
            return Result<CompanyPosition>.BadRequest("Cargo não pertence a esta empresa.");

        return Result<CompanyPosition>.Ok(position);
    }

    public async Task<Result> EnsureNoActiveMembersAsync(Guid companyId, IReadOnlyCollection<Guid> positionIds, CancellationToken ct)
    {
        var blocked = await _positionRepository.GetIdsWithActiveMembersAsync(companyId, positionIds, ct);

        if (blocked.Count > 0)
            return Result.BadRequest($"Não é possível desativar cargos com membros ativos. Cargos bloqueados: {string.Join(", ", blocked)}");

        return Result.Ok();
    }
}
