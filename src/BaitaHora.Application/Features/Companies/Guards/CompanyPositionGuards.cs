using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyPositionGuards : ICompanyPositionGuards
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyPositionGuards(ICompanyRepository companyRepository)
        => _companyRepository = companyRepository;

    public Result<CompanyPosition> ValidatePosition(Company company, Guid positionId, bool requireActive)
    {
        var position = company.Positions.SingleOrDefault(p => p.Id == positionId);

        if (position is null)
            return Result<CompanyPosition>.NotFound("Cargo não encontrado para esta empresa.");

        if (requireActive && !position.IsActive)
            return Result<CompanyPosition>.BadRequest("Cargo inativo.");

        return Result<CompanyPosition>.Ok(position);
    }

    public async Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForActivation(
        Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);
        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.BadRequest("Nenhum cargo informado.");

        var positions = await _companyRepository.GetPositionsByIdsAsync(companyId, ids, ct);

        var foundIds = positions.Select(p => p.Id).ToHashSet();
        var notFound = ids.Where(id => !foundIds.Contains(id)).ToArray();

        if (notFound.Length > 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.NotFound(
                $"Cargo(s) não encontrado(s) na empresa: {string.Join(", ", notFound)}");

        var inactive = positions.Where(p => !p.IsActive).ToList();

        if (inactive.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyPosition>>.Conflict(
                "Alguns cargos já estão ativos e não podem ser reativados.");

        return Result<IReadOnlyCollection<CompanyPosition>>.Ok(inactive);
    }

    public async Task<Result<IReadOnlyCollection<CompanyPosition>>> ValidatePositionsForDesactivation(
        Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);
        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.BadRequest("Nenhum cargo informado.");

        var positions = await _companyRepository.GetPositionsByIdsAsync(companyId, ids, ct);

        var foundIds = positions.Select(p => p.Id).ToHashSet();
        var notFound = ids.Where(id => !foundIds.Contains(id)).ToArray();

        if (notFound.Length > 0)
            return Result<IReadOnlyCollection<CompanyPosition>>.NotFound(
                $"Cargo(s) não encontrado(s) na empresa: {string.Join(", ", notFound)}");

        var active = positions.Where(p => p.IsActive).ToList();

        if (active.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyPosition>>.Conflict(
                "Alguns cargos já estão inativos e não podem ser desativados novamente.");

        return Result<IReadOnlyCollection<CompanyPosition>>.Ok(active);
    }
}