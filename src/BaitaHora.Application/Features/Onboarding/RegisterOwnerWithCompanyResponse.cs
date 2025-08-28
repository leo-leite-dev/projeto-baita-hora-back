namespace BaitaHora.Application.Features.Onboarding;

public sealed record RegisterOwnerWithCompanyResponse(
    Guid OwnerId,
    Guid CompanyId,
    string OwnerFullName,
    string OwnerCompanyName
);