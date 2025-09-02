using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;
using FluentValidation;
using BaitaHora.Application.Feature.Customers;

namespace BaitaHora.Application.Features.Customers.Create;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Nome do cliente é obrigatório.")
            .Must(v => Username.TryParse(v, out _))
            .WithMessage("Nome inválido. Deve ter entre 3 e 50 caracteres.");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Telefone é obrigatório.")
            .Must(v => Phone.TryParse(v, out _))
            .WithMessage("Telefone inválido.");

        RuleFor(x => x.CustomerCpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(v => CPF.TryParse(v, out _))
            .WithMessage("CPF inválido.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId é obrigatório.");
    }
}