using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Shared;

namespace BaitaHora.Application.Features.Companies.Guards;

public sealed class CompanyServiceOfferingGuards : ICompanyServiceOfferingGuards
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyServiceOfferingGuards(ICompanyRepository companyRepository)
        => _companyRepository = companyRepository;

    public Result<CompanyServiceOffering> ValidateServiceOffering( Company company, Guid serviceOfferingId, bool requireActive)
    {
        var service = company.ServiceOfferings
            .SingleOrDefault(s => s.Id == serviceOfferingId);

        if (service is null)
            return Result<CompanyServiceOffering>.NotFound("Serviço não encontrado para esta empresa.");

        if (requireActive && !service.IsActive)
            return Result<CompanyServiceOffering>.BadRequest("Serviço inativo.");

        return Result<CompanyServiceOffering>.Ok(service);
    }

    public async Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForActivation(
        Guid companyId, IEnumerable<Guid>? serviceOfferingIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(serviceOfferingIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.BadRequest("Nenhum serviço informado.");

        var services = await _companyRepository.GetServiceOfferingsByIdsAsync(companyId, ids, ct);

        var foundIds = services.Select(s => s.Id).ToHashSet();
        var notFound = ids.Where(id => !foundIds.Contains(id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.NotFound(
                $"Os serviços {string.Join(", ", notFound)} não foram encontrados.");

        var inactive = services.Where(s => !s.IsActive).ToList();
        if (inactive.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.Conflict(
                "Alguns serviços já estão ativos e não podem ser reativados.");

        return Result<IReadOnlyCollection<CompanyServiceOffering>>.Ok(inactive);
    }

    public async Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForDesactivation(
        Guid companyId, IEnumerable<Guid>? serviceOfferingIds, CancellationToken ct)
    {
        var ids = IdSet.Normalize(serviceOfferingIds);

        if (ids.Count == 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.BadRequest("Nenhum serviço informado.");

        var services = await _companyRepository.GetServiceOfferingsByIdsAsync(companyId, ids, ct);

        var foundIds = services.Select(s => s.Id).ToHashSet();
        var notFound = ids.Where(id => !foundIds.Contains(id)).ToList();
        if (notFound.Count > 0)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.NotFound(
                $"Os serviços {string.Join(", ", notFound)} não foram encontrados.");

        var active = services.Where(s => s.IsActive).ToList();
        if (active.Count != ids.Count)
            return Result<IReadOnlyCollection<CompanyServiceOffering>>.Conflict(
                "Alguns serviços já estão inativos e não podem ser desativados novamente.");

        return Result<IReadOnlyCollection<CompanyServiceOffering>>.Ok(active);
    }
}