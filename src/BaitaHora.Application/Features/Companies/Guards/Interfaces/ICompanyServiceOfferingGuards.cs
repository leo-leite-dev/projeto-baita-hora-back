using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyServiceOfferingGuards
{
    Result<CompanyServiceOffering> ValidateServiceOffering(Company company, Guid serviceOfferingId, bool requireActive);
    Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForActivation(Guid companyId, IEnumerable<Guid>? serviceOfferingIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForDesactivation(Guid companyId, IEnumerable<Guid>? serviceOfferingIds, CancellationToken ct);
}