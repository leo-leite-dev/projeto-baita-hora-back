using BaitaHora.Application.Features.Commons.Requests;

namespace BaitaHora.Application.Features.Companies.DTOs.Requests;

public sealed record CompanyRequest(
    string Name,
    string Cnpj,
    string? TradeName,
    string Email,
    string Phone,
    AddressRequest Address
);