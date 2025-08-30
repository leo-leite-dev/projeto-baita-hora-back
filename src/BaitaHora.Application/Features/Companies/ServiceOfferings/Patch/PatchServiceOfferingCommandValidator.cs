using FluentValidation;
using BaitaHora.Application.Features.Companies.ServiceOffering.Validators;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed class PatchServiceOfferingCommandValidator
    : ServiceOfferingValidatorBase<PatchServiceOfferingCommand>
{
    public PatchServiceOfferingCommandValidator()
        : base(
            required: false,
            companyIdSelector: x => x.CompanyId,
            nameSelector: x => x.ServiceOfferingName,
            amountSelector: x => x.Amount,
            currencySelector: x => x.Currency,
            serviceOfferingIdSelector: x => x.ServiceOfferingId
        )
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.ServiceOfferingName) ||
                x.Amount.HasValue ||
                !string.IsNullOrWhiteSpace(x.Currency)
            )
            .WithMessage("Nenhum campo para atualizar.");
    }
}