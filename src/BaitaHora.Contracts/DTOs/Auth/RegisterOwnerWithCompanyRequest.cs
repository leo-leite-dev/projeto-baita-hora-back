using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record RegisterOwnerWithCompanyRequest(
    CreateUserRequest Owner,
    CreateCompanyRequest Company
);