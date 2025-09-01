using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyServiceOfferingGuards
{
    Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForActivation(IEnumerable<Guid>? positionIds, CancellationToken ct);
    Task<Result<IReadOnlyCollection<CompanyServiceOffering>>> ValidateServiceOfferingsForDesactivation(Guid companyId, IEnumerable<Guid>? positionIds, CancellationToken ct);
}