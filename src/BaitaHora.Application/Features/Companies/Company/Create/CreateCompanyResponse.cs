namespace BaitaHora.Application.Features.Companies.CreateCompany;

public sealed record CreateCompanyResponse(
    Guid CompanyId,
    Guid OwnerId,
    string FullName,
    string CompanyName
);