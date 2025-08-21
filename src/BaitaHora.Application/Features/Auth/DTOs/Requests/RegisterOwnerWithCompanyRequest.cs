using BaitaHora.Application.Companies.DTOs.Requests;
using BaitaHora.Application.Users.DTOs;

namespace BaitaHora.Application.Auth.DTOs.Requests;

public sealed record RegisterOwnerWithCompanyRequest(
    UserRequest User,
    CompanyRequest Company
);