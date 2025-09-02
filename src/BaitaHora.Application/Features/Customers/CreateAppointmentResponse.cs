namespace BaitaHora.Application.Features.Customers.Create;

public sealed record CreateCustomerResponse(
    Guid CustomerId,
    string Name,
    string Phone,
    string Cpf
);