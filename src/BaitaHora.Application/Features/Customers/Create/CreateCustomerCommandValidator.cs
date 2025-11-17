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
            .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres.")
            .MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Telefone é obrigatório.")
            .Must(v => Phone.TryParse(v, out _))
            .WithMessage("Telefone inválido.");

        RuleFor(x => x.CustomerCpf)
            .NotEmpty().WithMessage("CPF é obrigatório.")
            .Must(v => CPF.TryParse(v, out _))
            .WithMessage("CPF inválido.");
    }
}