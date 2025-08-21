using BaitaHora.Application.Companies.DTOs;
using BaitaHora.Application.Companies.Inputs;
using BaitaHora.Application.Users.DTOs;

namespace BaitaHora.Application.Auth.Inputs;

public sealed record RegisterOwnerWithCompanyInput(
    UserInput User,
    CompanyInput Company
);