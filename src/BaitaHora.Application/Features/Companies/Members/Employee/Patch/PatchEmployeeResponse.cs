namespace BaitaHora.Application.Features.Companies.Responses;

public sealed record PatchEmployeeResponse(
    Guid MemberId,
    string Name
);