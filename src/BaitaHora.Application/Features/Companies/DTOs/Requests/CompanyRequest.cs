using BaitaHora.Application.Addresses.DTOs.Requests;

namespace BaitaHora.Application.Companies.DTOs.Requests;

public sealed record CompanyRequest(
    string Name,
    string Cnpj,
    string? TradeName,
    string Email,
    string Phone,
    AddressRequest Address
);