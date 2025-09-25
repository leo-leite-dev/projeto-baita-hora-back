using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyPositionGuards : ICompanyPositionGuards
{
    private readonly ICompanyPositionRepository _positionRepository;

    public CompanyPositionGuards(ICompanyPositionRepository positionRepository)
        => _positionRepository = positionRepository;


    public async Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForActivation(IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.BadRequest("Nenhum cargo informado.");

        var positions = await _positionRepository.GetByIdsAsync(ids, ct);

        var notFound = ids.Except(positions.Select(p => p.Id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.NotFound(
                $"Os cargos {string.Join(", ", notFound)} não foram encontrados.");

        var inactive = positions.Where(p => !p.IsActive).ToList();
        if (inactive.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyPosition>>.Conflict(
                "Alguns cargos já estão ativos e não podem ser reativados.");

        return Result<IReadOnlyCollection<CompanyPosition>>.Ok(inactive);
    }

    public async Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForDeactivation(IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.BadRequest("Nenhum cargo informado.");

        var positions = await _positionRepository.GetByIdsAsync(ids, ct);

        var notFound = ids.Except(positions.Select(p => p.Id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.NotFound(
                $"Os cargos {string.Join(", ", notFound)} não foram encontrados.");

        var active = positions.Where(p => p.IsActive).ToList();
        if (active.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyPosition>>.Conflict(
                "Alguns cargos já estão inativos e não podem ser desativados novamente.");

        var owners = active.Where(p => p.AccessLevel == CompanyRole.Owner).Select(p => p.Id).ToList();
        if (owners.Count > 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.Forbidden(
                $"Não é permitido desativar cargo(s) com papel Owner. IDs bloqueados: {string.Join(", ", owners)}");

        if (active.Any(p => p.Members.Any(m => m.IsActive)))
            return Result<IReadOnlyCollection<CompanyPosition>>.Conflict("Há cargos com membros ativos.");

        return Result<IReadOnlyCollection<CompanyPosition>>.Ok(active);
    }

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
}
