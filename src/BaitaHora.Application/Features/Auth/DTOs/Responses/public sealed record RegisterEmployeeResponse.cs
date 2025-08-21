namespace BaitaHora.Application.Feature.Auth.DTOs.Responses;

public sealed record RegisterEmployeeResponse
{
    public Guid EmployeeId { get; init; }
}