using FluentValidation;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get;

public sealed class ListServiceOfferingsValidator : AbstractValidator<ListServiceOfferingsQuery>
{
    public ListServiceOfferingsValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}