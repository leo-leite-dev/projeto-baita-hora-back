using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Api.Contracts.Companies.Requests;

public sealed record CreateCompanyPositionRequest(
    string PositionName,
    CompanyRole AccessLevel
);