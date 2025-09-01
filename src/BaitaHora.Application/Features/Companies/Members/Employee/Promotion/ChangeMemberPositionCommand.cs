using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Companies.Features.Members.Promotion;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.ChangePosition;

public sealed record ChangeMemberPositionCommand(
    Guid CompanyId,
    Guid EmployeeId,
    Guid NewPositionId,
    bool AlignRoleToPosition
) : IRequest<Result<ChangeMemberPositionResponse>>;
