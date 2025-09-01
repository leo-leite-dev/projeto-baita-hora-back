namespace BaitaHora.Application.Features.Companies.Employees.Disable;

public sealed record DisableEmployeesResponse(
    IReadOnlyCollection<Guid> EmployeeIds
);