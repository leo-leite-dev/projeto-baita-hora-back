using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record RegisterOwnerWithCompanyRequest(
    CreateUserRequest Owner,
    CreateCompanyRequest Company
);