using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record RegisterEmployeeRequest(
    Guid PositionId,
    CreateUserRequest Employee
);