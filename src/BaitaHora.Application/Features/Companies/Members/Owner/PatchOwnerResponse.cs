namespace BaitaHora.Application.Features.Companies.Members.Owner;

public sealed record PatchOwnerResponse(
    Guid OwnerId,
    string OwnerFullName
);