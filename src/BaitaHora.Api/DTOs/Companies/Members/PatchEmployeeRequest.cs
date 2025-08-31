using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record PatchEmployeeRequest(
    PatchUserRequest? Employee
);