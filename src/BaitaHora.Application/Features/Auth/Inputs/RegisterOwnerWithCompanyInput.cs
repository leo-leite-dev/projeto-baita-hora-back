using BaitaHora.Application.Features.Companies.Inputs;
using BaitaHora.Application.Features.Users.DTOs;

namespace BaitaHora.Application.Features.Auth.Inputs;

public sealed record RegisterOwnerWithCompanyInput(
    UserInput User,
    CompanyInput Company
);