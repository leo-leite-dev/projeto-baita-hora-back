using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Application.Features.Onboarding;

namespace BaitaHora.Application.Features.Companies.Common;

public static class CompanyAssembler
{
    public readonly record struct CompanyVO(
        string CompanyName,
        string? TradeName,
        CNPJ Cnpj,
        Email CompanyEmail,
        Phone CompanyPhone,
        Address Address
    );

    public static CompanyVO BuildCompanyVO(CreateCompanyWithOwnerCommand c)
    {
        var cnpj = CNPJ.Parse(c.Cnpj);
        var email = Email.Parse(c.CompanyEmail);
        var phone = Phone.Parse(c.CompanyPhone);

        var addr = Address.Parse(
            street: c.Address.Street,
            number: c.Address.Number,
            neighborhood: c.Address.Neighborhood,
            city: c.Address.City,
            state: c.Address.State,
            zipCode: c.Address.ZipCode,
            complement: c.Address.Complement
        );

        return new CompanyVO(c.CompanyName, c.TradeName, cnpj, email, phone, addr);
    }
}