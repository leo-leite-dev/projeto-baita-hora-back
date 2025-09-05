using FluentValidation;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

public sealed class GetServiceOfferingByIdValidator
    : AbstractValidator<GetServiceOfferingByIdQuery>
{
    public GetServiceOfferingByIdValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ServiceOfferingId).NotEmpty();
    }
}