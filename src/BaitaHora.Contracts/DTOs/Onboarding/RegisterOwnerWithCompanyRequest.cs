using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOS.Onbording;

public sealed record RegisterOwnerWithCompanyRequest(
    CreateUserRequest Owner,
    CreateCompanyRequest Company
);