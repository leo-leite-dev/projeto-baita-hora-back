using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOs.Companies;

public sealed record PatchEmployeeRequest(
    Guid? PositionId,
    PatchUserRequest? Employee
);