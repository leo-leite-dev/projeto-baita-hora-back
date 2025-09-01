using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable
{
    public sealed class DisableServiceOfferingsUseCase
    {
        private readonly ICompanyGuards _companyGuards;
        private readonly ICompanyServiceOfferingGuards _serviceOfferingGuards;

        public DisableServiceOfferingsUseCase(
            ICompanyGuards companyGuards,
            ICompanyServiceOfferingGuards serviceOfferingGuards)
        {
            _companyGuards = companyGuards;
            _serviceOfferingGuards = serviceOfferingGuards;
        }

        public async Task<Result<DisableServiceOfferingsResponse>> HandleAsync(
            DisableServiceOfferingsCommand cmd, CancellationToken ct)
        {

            var compRes = await _companyGuards.GetWithPositionsAndServiceOfferings(cmd.CompanyId, ct);
            if (compRes.IsFailure)
                return Result<DisableServiceOfferingsResponse>.FromError(compRes);

            var company = compRes.Value!;

            var valRes = await _serviceOfferingGuards
                .ValidateServiceOfferingsForDesactivation(cmd.CompanyId, cmd.ServiceOfferingIds, ct);
            if (valRes.IsFailure)
                return Result<DisableServiceOfferingsResponse>.FromError(valRes);

            var services = valRes.Value!;
            var ids = services.Select(s => s.Id).ToArray();

            company.DetachServiceOfferingsFromAllPositions(ids);

            foreach (var s in services)
                s.Deactivate();

            return Result<DisableServiceOfferingsResponse>.Ok(new(ids));
        }
    }
}