using FluentValidation;
using BaitaHora.Application.Features.Addresses.PatchAddress;

namespace BaitaHora.Application.Features.Companies.Common.Patch;

public sealed class PatchCompanyBaseCommandValidator
    : CompanyValidatorBase<PatchCompanyBaseCommand>
{
    public PatchCompanyBaseCommandValidator() : base(required: false)
    {
        RuleFor(x => x).Must(HasAnyField)
            .WithMessage("Envie ao menos um campo para atualizar a empresa.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!).SetValidator(new PatchAddressCommandValidator());
        });
    }

    private static bool HasAnyField(PatchCompanyBaseCommand x) =>
        x.CompanyName is not null ||
        x.Cnpj is not null ||
        x.TradeName is not null ||
        x.CompanyEmail is not null ||
        x.CompanyPhone is not null ||
        x.Address is not null;
}