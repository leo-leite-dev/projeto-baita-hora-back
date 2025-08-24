using BaitaHora.Contracts.DTOS.Auth;


namespace BaitaHora.Seeder.Builders;

public static class PayloadBuilder
{
    public static RegisterOwnerWithCompanyRequest BuildRegisterOwnerWithCompany()
    {
        var company = CompanyBuilder.Default();
        var owner = OwnerBuilder.Default();
        return new RegisterOwnerWithCompanyRequest(Owner: owner, Company: company);
    }
}