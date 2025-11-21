using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ListMembers.Get.ListAll;

public sealed class ListMembersHandler
    : IRequestHandler<ListMembersQuery, Result<IReadOnlyList<MemberDetails>>>
{
    private readonly ICompanyMemberRepository _memberRepository;
    private readonly ICurrentCompany _currentCompany;

    public ListMembersHandler(
        ICompanyMemberRepository memberRepository,
        ICurrentCompany currentCompany)
    {
        _memberRepository = memberRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<IReadOnlyList<MemberDetails>>> Handle(
        ListMembersQuery request, CancellationToken ct)
    {
        var list = await _memberRepository.ListAllMembersByCompanyAsync(_currentCompany.Id, ct);
        return Result<IReadOnlyList<MemberDetails>>.Ok(list);
    }
}