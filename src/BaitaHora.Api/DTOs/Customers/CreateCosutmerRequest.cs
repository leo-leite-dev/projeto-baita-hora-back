namespace BaitaHora.Contracts.DTOs.Customers;

public sealed record CreateCustomerRequest(
    string CustomerName,
    string CustomerPhone,
    string CustomerCpf
);