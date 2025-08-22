using BaitaHora.Api.Contracts.Companies.Requests;
using BaitaHora.Api.Contracts.Users.Requests;

namespace BaitaHora.Api.Contracts.Auth.Requests;

public sealed record RegisterOwnerWithCompanyRequest(
    UserRequest Owner,
    CompanyRequest Company
);