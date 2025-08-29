namespace BaitaHora.Application.Features.Companies.Common;

public interface ICompanyLike
{
    string? CompanyName { get; }
    string? Cnpj { get; }
    string? TradeName { get; }
    string? CompanyEmail { get; }
    string? CompanyPhone { get; }
}