using FluentValidation;
using BaitaHora.Application.Features.Companies.Common;
using BaitaHora.Application.Features.Commons.Validators;
using BaitaHora.Domain.Companies.ValueObjects;

public abstract class CompanyValidatorBase<T> : AbstractValidator<T> where T : ICompanyLike
{
    protected CompanyValidatorBase(bool required)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode  = CascadeMode.Stop;

        When(x => required || x.CompanyName is not null, () =>
        {
            var r = RuleFor(x => x.CompanyName!);
            if (required)
                r.NotEmpty().WithMessage("O nome da empresa é obrigatório.");

            r.MinimumLength(3).WithMessage("O nome da empresa deve ter pelo menos 3 caracteres.")
             .MaximumLength(120).WithMessage("O nome da empresa deve ter no máximo 120 caracteres.")
             .Matches(@"^[\p{L}\p{N}\s\-\&\.\,]+$")
                .WithMessage("O nome da empresa contém caracteres inválidos.");
        });

        When(x => x.TradeName is not null, () =>
        {
            RuleFor(x => x.TradeName!)
                .MaximumLength(120).WithMessage("O nome fantasia deve ter no máximo 120 caracteres.");
        });

        When(x => required || x.Cnpj is not null, () =>
        {
            var r = RuleFor(x => x.Cnpj!);
            if (required)
                r.NotEmpty().WithMessage("O CNPJ é obrigatório.");
            r.Must(c => c is not null && CNPJ.TryParse(c, out _))
                .WithMessage("CNPJ inválido. Verifique se possui 14 dígitos numéricos.");
        });

        When(x => required || x.CompanyEmail is not null, () =>
        {
            var r = RuleFor(x => x.CompanyEmail!);
            if (required)
                r.NotEmpty().WithMessage("O email é obrigatório.");
            r.EmailVo();
        });

        When(x => required || x.CompanyPhone is not null, () =>
        {
            var r = RuleFor(x => x.CompanyPhone!);
            if (required)
                r.NotEmpty().WithMessage("O telefone é obrigatório.");
            r.PhoneVo("+55");
        });
    }
}
