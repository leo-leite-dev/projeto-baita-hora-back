namespace BaitaHora.Contracts.DTOs.Companies;

public sealed record CreateCompanyServiceRequest(
    string ServiceName,
    decimal Amount,
    string Currency,
    IReadOnlyCollection<Guid> PositionIds
);