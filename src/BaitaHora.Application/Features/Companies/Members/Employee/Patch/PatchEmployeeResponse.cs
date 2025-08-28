namespace BaitaHora.Application.Features.Companies.Responses;

public sealed record PatchEmployeeResponse(
    Guid EmployeeId,
    string EmployeeName
);