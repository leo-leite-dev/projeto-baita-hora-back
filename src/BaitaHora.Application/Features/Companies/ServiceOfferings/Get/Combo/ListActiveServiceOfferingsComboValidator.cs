using FluentValidation;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed class ListActiveServiceOfferingsComboValidator
    : AbstractValidator<ListActiveServiceOfferingsComboQuery>
{
    public ListActiveServiceOfferingsComboValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Take).InclusiveBetween(1, 100);
    }
}