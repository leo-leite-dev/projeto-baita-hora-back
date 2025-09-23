namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record ActivateEmployeesRequest(IEnumerable<Guid> EmployeeIds);