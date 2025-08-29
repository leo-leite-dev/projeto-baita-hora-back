using FluentValidation;
using BaitaHora.Application.Features.Addresses.Create;

namespace BaitaHora.Application.Features.Companies.Common.Create;

public sealed class CreateCompanyBaseCommandValidator
    : CompanyValidatorBase<CreateCompanyBaseCommand>
{
    public CreateCompanyBaseCommandValidator() : base(required: true)
    {
        RuleFor(x => x.Address)
            .NotNull().WithMessage("O endereço da empresa é obrigatório.")
            .SetValidator(new CreateAddressCommandValidator()); 
    }
}