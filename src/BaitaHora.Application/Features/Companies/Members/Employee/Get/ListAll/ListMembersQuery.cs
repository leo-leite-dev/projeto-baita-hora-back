using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ListMembers.Get.ListAll;

public sealed record ListMembersQuery()
    : IRequest<Result<IReadOnlyList<MemberDetails>>>;
