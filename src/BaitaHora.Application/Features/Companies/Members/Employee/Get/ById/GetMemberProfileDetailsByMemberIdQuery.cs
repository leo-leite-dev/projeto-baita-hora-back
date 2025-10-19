using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using MediatR;

public sealed record GetMemberProfileDetailsByMemberIdQuery(Guid MemberId)
    : IRequest<Result<MemberProfileDetails>>;
