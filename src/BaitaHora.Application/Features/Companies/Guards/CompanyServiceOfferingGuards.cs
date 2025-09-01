using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyServiceOfferingGuards : ICompanyServiceOfferingGuards
{
    private readonly ICompanyServiceOfferingRepository _serviceOfferingRepository;

    public CompanyServiceOfferingGuards(ICompanyServiceOfferingRepository serviceOfferingRepository)
        => _serviceOfferingRepository = serviceOfferingRepository;


    public async Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForActivation(IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.BadRequest("Nenhum serviço informado.");

        var services = await _serviceOfferingRepository.GetByIdsAsync(ids, ct);

        var notFound = ids.Except(services.Select(p => p.Id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.NotFound(
                $"Os serviços {string.Join(", ", notFound)} não foram encontrados.");

        var inactive = services.Where(p => !p.IsActive).ToList();
        if (inactive.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.Conflict(
                "Alguns serviços já estão ativos e não podem ser reativados.");

        return Result<IReadOnlyCollection<CompanyServiceOffering>>.Ok(inactive);
    }

    public async Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForDesactivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(positionIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.BadRequest("Nenhum serviço informado.");

        var services = await _serviceOfferingRepository.GetByIdsAsync(ids, ct);

        var notFound = ids.Except(services.Select(p => p.Id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.NotFound(
                $"Os serviços {string.Join(", ", notFound)} não foram encontrados.");

        var active = services.Where(p => p.IsActive).ToList();
        if (active.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.Conflict(
                "Alguns serviços já estão inativos e não podem ser desativados novamente.");

        return Result<IReadOnlyCollection<CompanyServiceOffering>>.Ok(active);
    }
}