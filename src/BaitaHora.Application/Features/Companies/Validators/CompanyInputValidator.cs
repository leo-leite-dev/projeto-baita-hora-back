using BaitaHora.Application.Features.Commons.Validators;
using BaitaHora.Application.Features.Companies.Inputs;
using BaitaHora.Domain.Companies.ValueObjects;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Validators;

public sealed class CompanyInputValidator : AbstractValidator<CompanyInput>
{
    public CompanyInputValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("O nome da empresa é obrigatório.")
            .MinimumLength(2).WithMessage("O nome da empresa deve ter pelo menos 2 caracteres.")
            .MaximumLength(200).WithMessage("O nome da empresa deve ter no máximo 200 caracteres.")
            .Must(n => CompanyName.TryParse(n, out _))
                .WithMessage("O nome da empresa contém caracteres inválidos.");

        RuleFor(x => x.TradeName)
            .MaximumLength(200).WithMessage("O nome fantasia deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Must(c => CNPJ.TryParse(c, out _))
                .WithMessage("CNPJ inválido. Verifique se possui 14 dígitos numéricos.");

        RuleFor(x => x.Email).EmailVo();        
        RuleFor(x => x.Phone).PhoneVo("+55");   

        RuleFor(x => x.Address)
            .NotNull().WithMessage("O endereço da empresa é obrigatório.")
            .SetValidator(new AddressInputValidator());
    }
}