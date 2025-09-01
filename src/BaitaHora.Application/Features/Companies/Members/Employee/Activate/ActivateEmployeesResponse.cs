namespace BaitaHora.Application.Features.Companies.Employees.Activate;

public sealed record ActivateEmployeesResponse(
    IReadOnlyCollection<Guid> EmployeeIds
);