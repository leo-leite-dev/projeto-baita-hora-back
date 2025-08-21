namespace BaitaHora.Application.Auth.DTO.Responses;

public sealed record RegisterEmployeeResponse
{
    public Guid EmployeeId { get; init; }
}