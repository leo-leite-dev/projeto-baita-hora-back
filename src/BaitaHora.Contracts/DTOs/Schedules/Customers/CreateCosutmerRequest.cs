namespace BaitaHora.Contracts.DTOs.Schedules.Customers;

public sealed record CreateCustomerRequest(
    string CustomerName,
    string CustomerPhone,
    string CustomerCpf
);