namespace BaitaHora.Contracts.DTOs.Companies.Members;

public sealed record DisableEmployeesRequest(IEnumerable<Guid> EmployeeIds);