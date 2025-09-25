using BaitaHora.Application.Features.Companies.ServiceOffering.Validators;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Create;

public sealed class CreateServiceOfferingCommandValidator
    : ServiceOfferingValidatorBase<CreateServiceOfferingCommand>
{
    public CreateServiceOfferingCommandValidator()
        : base(
            required: true,
            nameSelector: x => x.ServiceOfferingName,
            amountSelector: x => x.Amount,
            currencySelector: x => x.Currency
        )
    { }
}