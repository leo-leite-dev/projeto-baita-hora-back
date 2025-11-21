using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Get.Options;

public sealed record ListMembersOptionsQuery(
    string? Search = null,
    int Take = 20
) : IRequest<Result<IReadOnlyList<MemberOptions>>>;