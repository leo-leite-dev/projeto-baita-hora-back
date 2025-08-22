using BaitaHora.Api.Contracts.Users.Requests;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Api.Contracts.Auth.Requests;

public sealed record RegisterEmployeeRequest(
    Guid PositionId,
    UserRequest Employee
    );