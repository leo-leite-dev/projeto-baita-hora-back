using BaitaHora.Application.Features.Companies.DTOs.Requests;
using BaitaHora.Application.Features.Users.DTOs;

namespace BaitaHora.Application.Features.Auth.DTOs.Requests;

public sealed record RegisterOwnerWithCompanyRequest(
    UserRequest User,
    CompanyRequest Company
);