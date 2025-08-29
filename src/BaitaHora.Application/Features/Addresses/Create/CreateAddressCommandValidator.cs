using DomainAddress = BaitaHora.Domain.Features.Common.ValueObjects.Address;
using FluentValidation;

namespace BaitaHora.Application.Features.Addresses.Create;

public sealed class CreateAddressCommandValidator 
    : AddressValidatorBase<CreateAddressCommand>
{
    public CreateAddressCommandValidator() : base(required: true)
    {
        RuleFor(x => x).Custom((x, ctx) =>
        {
            var ok = DomainAddress.TryParse(
                x.Street, x.Number, x.Complement, x.Neighborhood,
                x.City, x.State, x.ZipCode, out _, out var errors);

            if (!ok) foreach (var e in errors) ctx.AddFailure(e.Message);
        });
    }
}