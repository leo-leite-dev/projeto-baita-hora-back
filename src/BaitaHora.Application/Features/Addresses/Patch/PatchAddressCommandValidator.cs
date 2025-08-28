using BaitaHora.Application.Features.Addresses.Common;
using FluentValidation;
using DomainAddress = BaitaHora.Domain.Features.Common.ValueObjects.Address;

namespace BaitaHora.Application.Features.Addresses.PatchAddress;

public sealed class PatchAddressCommandValidator
    : AddressValidatorBase<PatchAddressCommand>
{
    public PatchAddressCommandValidator() : base(required: false)
    {
        RuleFor(x => x).Must(HasAnyField)
            .WithMessage("Envie ao menos um campo para atualizar o endereço.");

        When(AllRequiredProvided, () =>
        {
            RuleFor(x => x).Custom((x, ctx) =>
            {
                var ok = DomainAddress.TryParse(
                    x.Street!, x.Number!, x.Complement, x.Neighborhood!,
                    x.City!, x.State!, x.ZipCode!, out _, out var errors);

                if (!ok) foreach (var e in errors) ctx.AddFailure(e.Message);
            });
        });
    }

    private static bool HasAnyField(IAddressLike x) =>
        x.Street is not null || x.Number is not null || x.Complement is not null ||
        x.Neighborhood is not null || x.City is not null || x.State is not null || x.ZipCode is not null;

    private static bool AllRequiredProvided(IAddressLike x) =>
        x.Street is not null && x.Number is not null && x.Neighborhood is not null &&
        x.City is not null && x.State is not null && x.ZipCode is not null;
}